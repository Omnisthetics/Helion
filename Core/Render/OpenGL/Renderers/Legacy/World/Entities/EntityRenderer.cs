using System;
using System.Collections.Generic;
using GlmSharp;
using Helion.Geometry.Vectors;
using Helion.Graphics.Palettes;
using Helion.Render.OpenGL.Renderers.Legacy.World.Data;
using Helion.Render.OpenGL.Shared;
using Helion.Render.OpenGL.Shared.World.ViewClipping;
using Helion.Render.OpenGL.Texture.Legacy;
using Helion.Resources;
using Helion.Util.Configs;
using Helion.World;
using Helion.World.Entities;
using Helion.World.Geometry.Sectors;
using OpenTK.Graphics.OpenGL;

namespace Helion.Render.OpenGL.Renderers.Legacy.World.Entities;

public class EntityRenderer : IDisposable
{
    private readonly IConfig m_config;
    private readonly LegacyGLTextureManager m_textureManager;
    private readonly EntityProgram m_program = new();
    private readonly RenderDataManager<EntityVertex> m_dataManager;
    private readonly Dictionary<Vec2D, int> m_renderPositions = new(1024, new Vec2DCompararer());
    private double m_tickFraction;
    private Vec2F m_viewRightNormal;
    private TransferHeightView m_transferHeightView = TransferHeightView.Middle;
    private bool m_spriteAlpha;
    private bool m_spriteClip;
    private bool m_spriteZCheck;
    private bool m_alwaysFlood;
    private int m_spriteClipMin;
    private float m_spriteClipFactorMax;
    private bool m_disposed;

    public EntityRenderer(IConfig config, LegacyGLTextureManager textureManager)
    {
        m_config = config;
        m_textureManager = textureManager;
        m_dataManager = new(m_program);
        m_spriteAlpha = m_config.Render.SpriteTransparency;
        m_spriteClip = m_config.Render.SpriteClip;
        m_spriteZCheck = m_config.Render.SpriteZCheck;
        m_spriteClipMin = m_config.Render.SpriteClipMin;
        m_alwaysFlood = m_config.Render.AlwaysFloodFillFlats;
        m_spriteClipFactorMax = (float)m_config.Render.SpriteClipFactorMax;
    }

    ~EntityRenderer()
    {
        PerformDispose();
    }

    public void UpdateTo(IWorld world)
    {
        m_alwaysFlood = world.Config.Render.AlwaysFloodFillFlats;
    }
    
    public void Clear(IWorld world)
    {
        m_dataManager.Clear();
        m_renderPositions.Clear();
        m_spriteAlpha = m_config.Render.SpriteTransparency;
        m_spriteClip = m_config.Render.SpriteClip;
        m_spriteZCheck = m_config.Render.SpriteZCheck;
        m_spriteClipMin = m_config.Render.SpriteClipMin;
        m_spriteClipFactorMax = (float)m_config.Render.SpriteClipFactorMax;
    }

    public void SetTickFraction(double tickFraction) =>
        m_tickFraction = tickFraction;

    private static uint CalculateRotation(uint viewAngle, uint entityAngle)
    {
        // The rotation angle in diamond angle format. This is equal to 180
        // degrees + 22.5 degrees. See <see cref="CalculateRotation"/> docs
        // for more information.
        const uint SpriteFrameRotationAngle = 9 * (uint.MaxValue / 16);

        // This works as follows:
        //
        // First we find the angle that we have to the entity. Since
        // facing along with the actor (ex: looking at their back) wants to
        // give us the opposite rotation side, we add 180 degrees to our
        // angle delta.
        //
        // Then we add 22.5 degrees to that as well because we don't want
        // a transition when we hit 180 degrees... we'd rather have ranges
        // of [180 - 22.5, 180 + 22.5] be the angle rather than the range
        // [180 - 45, 180].
        //
        // Then we can do a bit shift trick which converts the higher order
        // three bits into the angle rotation between 0 - 7.
        return unchecked((viewAngle - entityAngle + SpriteFrameRotationAngle) >> 29);
    }

    private float GetOffsetZ(Entity entity, GLLegacyTexture texture)
    {
        float offsetAmount = texture.Offset.Y - texture.Height;
        if (offsetAmount >= 0 || entity.Definition.Flags.Missile)
            return offsetAmount;

        if (m_alwaysFlood || entity.Sector.Flood || entity.Sector.Floor.NoRender)
            return offsetAmount;

        if (!m_spriteClip)
            return 0;

        if (texture.Height < m_spriteClipMin || entity.Definition.IsInventory)
            return 0;

        if (entity.Position.Z - entity.HighestFloorSector.Floor.Z < texture.Offset.Y)
        {
            float maxHeight = (texture.Height - texture.BlankRowsFromBottom) * m_spriteClipFactorMax;
            if (-offsetAmount > maxHeight)
                offsetAmount = -maxHeight - texture.BlankRowsFromBottom;
            // Truncate to integer pixel amount. This helps the jumpiness for the stock large torches.
            return (int)offsetAmount;
        }

        return offsetAmount;
    }

    public unsafe void RenderEntity(Entity entity, in Vec3D position)
    {
        const double NudgeFactor = 0.0001;
        
        Vec3D centerBottom = entity.Position;
        Vec2D entityPos = centerBottom.XY;
        Vec2D position2D = position.XY;
        Vec2D nudgeAmount = Vec2D.Zero;

        SpriteDefinition? spriteDef = m_textureManager.GetSpriteDefinition(entity.Frame.SpriteIndex);
        uint rotation = 0;
        if (spriteDef != null && spriteDef.HasRotations)
        {
            uint viewAngle = ViewClipper.ToDiamondAngle(position2D, entityPos);
            uint entityAngle = ViewClipper.DiamondAngleFromRadians(entity.AngleRadians);
            rotation = CalculateRotation(viewAngle, entityAngle);
        }

        if (m_spriteZCheck)
        {
            Vec2D positionLookup = centerBottom.XY;
            if (m_renderPositions.TryGetValue(positionLookup, out int count))
            {
                double nudge = Math.Clamp(NudgeFactor * entityPos.Distance(position2D), NudgeFactor, double.MaxValue);
                nudgeAmount = Vec2D.UnitCircle(position.Angle(centerBottom)) * nudge * count;
                m_renderPositions[positionLookup] = count + 1;
            }
            else
            {
                m_renderPositions[positionLookup] = 1;
            }
        }

        SpriteRotation spriteRotation = m_textureManager.NullSpriteRotation;
        if (spriteDef != null)
            spriteRotation = m_textureManager.GetSpriteRotation(spriteDef, entity.Frame.Frame, rotation);
        GLLegacyTexture texture = (spriteRotation.Texture.RenderStore as GLLegacyTexture) ?? m_textureManager.NullTexture;
        Sector sector = entity.Sector.GetRenderSector(m_transferHeightView);

        short lightLevel = entity.Flags.Bright || entity.Frame.Properties.Bright ? (short)255 :
            (short)((sector.TransferFloorLightSector.LightLevel + sector.TransferCeilingLightSector.LightLevel) / 2);

        int halfTexWidth = texture.Dimension.Width / 2;
        float offsetZ = GetOffsetZ(entity, texture);
        // Multiply the X offset by the rightNormal X/Y to move the sprite according to the player's view
        // Doom graphics are drawn left to right and not centered
        var pos = new Vec3F((float)(entity.Position.X - nudgeAmount.X) - (m_viewRightNormal.X * texture.Offset.X) + (m_viewRightNormal.X * halfTexWidth), 
            (float)(entity.Position.Y - nudgeAmount.Y) - (m_viewRightNormal.Y * texture.Offset.X) + (m_viewRightNormal.Y * halfTexWidth), (float)entity.Position.Z + offsetZ);

        var prevPos = entity.Position == entity.PrevPosition ? pos : 
            new Vec3F(pos.X - (float)(entity.Position.X - entity.PrevPosition.X), 
            pos.Y - (float)(entity.Position.Y - entity.PrevPosition.Y),
            pos.Z - (float)(entity.Position.Z - entity.PrevPosition.Z));

        bool useAlpha = entity.Flags.Shadow || (m_spriteAlpha && entity.Alpha < 1.0f);
        RenderData<EntityVertex> renderData = useAlpha ? m_dataManager.GetAlpha(texture) : m_dataManager.GetNonAlpha(texture);
        float alpha = useAlpha ? entity.Alpha : 1.0f;
        float flipU = spriteRotation.Mirror ? 1.0f : 0.0f;
        float fuzz = 0.0f;
        if (entity.Flags.Shadow)
        {
            fuzz = 1.0f;
            alpha = 0.99f;
        }

        int newLength = renderData.Vbo.Data.Length + 1;
        renderData.Vbo.Data.EnsureCapacity(newLength);
        fixed (EntityVertex* vertex = &renderData.Vbo.Data.Data[renderData.Vbo.Data.Length])
        {
            vertex->Pos = pos;
            vertex->PrevPos = prevPos;
            vertex->LightLevel = lightLevel;
            vertex->Alpha = alpha;
            vertex->Fuzz = fuzz;
            vertex->FlipU = flipU;
        }
        renderData.Vbo.Data.SetLength(newLength);
    }

    private void SetUniforms(RenderInfo renderInfo)
    {
        m_viewRightNormal = renderInfo.Camera.Direction.XY.RotateRight90().Unit();
        m_transferHeightView = renderInfo.TransferHeightView;
        m_program.BoundTexture(TextureUnit.Texture0);
        m_program.ExtraLight(renderInfo.Uniforms.ExtraLight);
        m_program.HasInvulnerability(renderInfo.Uniforms.DrawInvulnerability);
        m_program.LightLevelMix(renderInfo.Uniforms.Mix);
        m_program.Mvp(renderInfo.Uniforms.Mvp);
        m_program.MvpNoPitch(renderInfo.Uniforms.MvpNoPitch);
        m_program.FuzzFrac(renderInfo.Uniforms.TimeFrac);
        m_program.TimeFrac(renderInfo.TickFraction);
        m_program.ViewRightNormal(m_viewRightNormal);
        m_program.DistanceOffset(Renderer.GetDistanceOffset(renderInfo));
        m_program.ColorMix(renderInfo.Uniforms.ColorMix);
    }

    public void RenderAlpha(RenderInfo renderInfo)
    {
        Render(renderInfo, true);
    }

    public void RenderNonAlpha(RenderInfo renderInfo)
    {
        Render(renderInfo, false);
    }
    
    private void Render(RenderInfo renderInfo, bool alpha)
    {
        m_tickFraction = renderInfo.TickFraction;
        m_program.Bind();
        GL.ActiveTexture(TextureUnit.Texture0);
        SetUniforms(renderInfo);

        if (alpha)
            m_dataManager.RenderAlpha(PrimitiveType.Points);
        else
            m_dataManager.RenderNonAlpha(PrimitiveType.Points);

        m_program.Unbind();
    }

    public void ResetInterpolation(IWorld world)
    {
        Clear(world);
    }
    
    private void PerformDispose()
    {
        if (m_disposed)
            return;
        
        m_program.Dispose();
        m_dataManager.Dispose();

        m_disposed = true;
    }

    public void Dispose()
    {
        PerformDispose();
        GC.SuppressFinalize(this);
    }
    
    private class Vec2DCompararer : IEqualityComparer<Vec2D>
    {
        public bool Equals(Vec2D x, Vec2D y) => x.X == y.X && x.Y == y.Y;
        public int GetHashCode(Vec2D obj) => HashCode.Combine(obj.X, obj.Y);
    }
}
