﻿using Helion.Geometry;
using Helion.Geometry.Boxes;
using Helion.Geometry.Vectors;
using Helion.Render.Common.Textures;

namespace Helion.Render.OpenGL.Textures
{
    /// <summary>
    /// A handle to a texture. Such a handle may either be the entire texture,
    /// or is a component of a larger texture (such as in a texture atlas).
    /// </summary>
    public class GLTextureHandle : IRenderableTextureHandle
    {
        public readonly string Name;
        public int Index { get; }
        public int LayerIndex { get; }
        public Box2I Area { get; }
        public Box2F UV { get; }
        public Vec2I Offset { get; }
        public Dimension Dimension => Area.Dimension;
        public readonly GLTexture Texture;

        public GLTextureHandle(string name, int index, int layerIndex, Box2I area, Box2F uv, Vec2I offset, 
            GLTexture texture)
        {
            Name = name;
            Index = index;
            LayerIndex = layerIndex;
            Texture = texture;
            Area = area;
            UV = uv;
            Offset = offset;
        }
    }
}
