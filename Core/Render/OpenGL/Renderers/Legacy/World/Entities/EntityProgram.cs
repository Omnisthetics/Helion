﻿using GlmSharp;
using Helion.Geometry.Vectors;
using Helion.Render.OpenGL.Shader;
using OpenTK.Graphics.OpenGL;

namespace Helion.Render.OpenGL.Renderers.Legacy.World.Entities;

public class EntityProgram : RenderProgram
{
    public EntityProgram() : base("Entity")
    {
    }
    
    public void BoundTexture(TextureUnit unit) => Uniforms.Set(unit, "boundTexture");
    public void ExtraLight(int extraLight) => Uniforms.Set(extraLight, "extraLight");
    public void HasInvulnerability(bool invul) => Uniforms.Set(invul, "hasInvulnerability");
    public void LightDropoff(bool dropoff) => Uniforms.Set(dropoff, "lightDropoff");
    public void LightLevelMix(float lightLevelMix) => Uniforms.Set(lightLevelMix, "lightLevelMix");
    public void Mvp(mat4 mvp) => Uniforms.Set(mvp, "mvp");
    public void MvpNoPitch(mat4 mvpNoPitch) => Uniforms.Set(mvpNoPitch, "mvpNoPitch");
    public void TimeFrac(float frac) => Uniforms.Set(frac, "timeFrac");
    public void ViewRightNormal(Vec2F viewRightNormal) => Uniforms.Set(viewRightNormal, "viewRightNormal");

    protected override string? VertexShader() => @"
        #version 330

        layout(location = 0) in vec3 pos;
        layout(location = 1) in float lightLevel;
        layout(location = 2) in float alpha;
        layout(location = 3) in int flags;

        out float lightLevelOut;
        out float alphaOut; 
        out int flagsOut;

        void main()
        {
            lightLevelOut = lightLevel * 256;
            alphaOut = alpha;
            flagsOut = flags;

            gl_Position = vec4(pos, 1);
        }
    ";

    protected override string? GeometryShader() => @"
        #version 330 core

        layout(points) in;
        layout(triangle_strip, max_vertices = 4) out;

        const int FuzzBit = 1;
        const int FlipUBit = 2;

        in float lightLevelOut[];
        in float alphaOut[]; 
        in int flagsOut[];

        out vec2 uvFrag;
        out float distFrag;
        flat out float lightLevelFrag;
        flat out float alphaFrag;
        flat out int fuzzFrag;

        uniform mat4 mvp;
        uniform mat4 mvpNoPitch;
        uniform vec2 viewRightNormal;
        uniform sampler2D boundTexture;

        void main()
        {
            int isFuzzValue = 0;
            int isFuzzValueResult = flagsOut[0] & FuzzBit;
            if (isFuzzValueResult > 0)
                isFuzzValue = 1;

            float leftU = 0.0;
            float rightU = 1.0;
            int isFlipUBitResult = flagsOut[0] & FlipUBit;
            if (isFlipUBitResult > 0)
            {
                leftU = 1.0;
                rightU = 0.0;
            }

            vec3 pos = gl_in[0].gl_Position.xyz;
            ivec2 textureDim = textureSize(boundTexture, 0);
            float halfTexWidth = textureDim.x * 0.5;
            vec3 posMoveDir = vec3(viewRightNormal, 0);
            vec3 minPos = pos - (posMoveDir * halfTexWidth);
            vec3 maxPos = pos + (posMoveDir * halfTexWidth) + (vec3(0, 0, 1) * textureDim.y);

            // Triangle strip ordering is: v0 v1 v2, v2 v1 v3
            // We also need to be going counter-clockwise.
            // Also the UV's are inverted, so draw from 1 down to 0 along the Y.

            gl_Position = mvp * vec4(minPos.x, minPos.y, minPos.z, 1);
            distFrag = (mvpNoPitch * vec4(minPos.x, minPos.y, minPos.z, 1)).z;
            uvFrag = vec2(leftU, 1);
            lightLevelFrag = lightLevelOut[0];
            alphaFrag = alphaOut[0];
            fuzzFrag = isFuzzValue;
            EmitVertex();

            gl_Position = mvp * vec4(maxPos.x, maxPos.y, minPos.z, 1);
            distFrag = (mvpNoPitch * vec4(maxPos.x, maxPos.y, minPos.z, 1)).z;
            uvFrag = vec2(rightU, 1);
            lightLevelFrag = lightLevelOut[0];
            alphaFrag = alphaOut[0];
            fuzzFrag = isFuzzValue;
            EmitVertex();

            gl_Position = mvp * vec4(minPos.x, minPos.y, maxPos.z, 1);
            distFrag = (mvpNoPitch * vec4(minPos.x, minPos.y, maxPos.z, 1)).z;
            uvFrag = vec2(leftU, 0);
            lightLevelFrag = lightLevelOut[0];
            alphaFrag = alphaOut[0];
            fuzzFrag = isFuzzValue;
            EmitVertex();

            gl_Position = mvp * vec4(maxPos.x, maxPos.y, maxPos.z, 1);
            distFrag = (mvpNoPitch * vec4(maxPos.x, maxPos.y, maxPos.z, 1)).z;
            uvFrag = vec2(rightU, 0);
            lightLevelFrag = lightLevelOut[0];
            alphaFrag = alphaOut[0];
            fuzzFrag = isFuzzValue;
            EmitVertex();
    
            EndPrimitive();
        }  
    ";

    protected override string? FragmentShader() => @"
        #version 330

        in vec2 uvFrag;
        in float distFrag;
        flat in float lightLevelFrag;
        flat in float alphaFrag;
        flat in int fuzzFrag;

        out vec4 fragColor;

        uniform int hasInvulnerability;
        uniform int lightDropoff;
        uniform float timeFrac;
        uniform sampler2D boundTexture;
        uniform float lightLevelMix;
        uniform int extraLight;

        // These two functions are found here:
        // https://gist.github.com/patriciogonzalezvivo/670c22f3966e662d2f83
        float rand(vec2 n)
        {
            return fract(sin(dot(n, vec2(12.9898, 4.1414))) * 43758.5453);
        }

        float noise(vec2 p)
        {
            vec2 ip = floor(p);
            vec2 u = fract(p);
            u = u * u * (3.0 - 2.0 * u);

            float res = mix(
	            mix(rand(ip), rand(ip + vec2(1.0, 0.0)), u.x),
	            mix(rand(ip + vec2(0.0, 1.0)), rand(ip + vec2(1.0, 1.0)), u.x), u.y);
            return res * res;
        }

        // Defined in GLHelper as well
        const int colorMaps = 32;
        const int colorMapClamp = 31;
        const int scaleCount = 16;
        const int scaleCountClamp = 15;
        const int maxLightScale = 23;
        const int lightFadeStart = 56;

        float calculateLightLevel(float lightLevel)
        {
            if (lightLevel <= 0.75)
            {
                if (lightLevel > 0.4)
                {
	                lightLevel = -0.6375 + (1.85 * lightLevel);
	                if (lightLevel < 0.08)
                    {
		                lightLevel = 0.08 + (lightLevel * 0.2);
	                }
                } 
                else
                {
	                lightLevel /= 5.0;
                }
            }
            return lightLevel;
        }

        void main()
        {
            float lightLevel = lightLevelFrag;

            if (lightDropoff > 0)
            {
                float d = clamp(distFrag - lightFadeStart, 0, distFrag);
                int sub = int(21.53536 - 21.63471881/(1 + pow((d/48.46036), 0.9737408)));
                int index = clamp(int(lightLevel / scaleCount), 0, scaleCountClamp);
                sub = maxLightScale - clamp(sub - extraLight, 0, maxLightScale);
                index = clamp(((scaleCount - index - 1) * 2 * colorMaps/scaleCount) - sub, 0, colorMapClamp);
                lightLevel = float(colorMaps - index) / colorMaps;
            }
            else
            {
                lightLevel += extraLight * 8;
                lightLevel = calculateLightLevel(lightLevel / 256.0);
            }

            lightLevel = mix(clamp(lightLevel, 0.0, 1.0), 1.0, lightLevelMix);
            fragColor = texture(boundTexture, uvFrag.st);

            if (fuzzFrag > 0)
            {
                lightLevel = 0;
                // The division/floor is to chunk pixels together to make
                // blocks. A larger denominator makes it more blocky.
                vec2 blockCoordinate = floor(gl_FragCoord.xy);
                fragColor.w *= step(0.25, noise(blockCoordinate * timeFrac));
            }

            fragColor.xyz *= lightLevel;
            fragColor.w *= alphaFrag;

            if (fragColor.w <= 0.0)
                discard;

            // If invulnerable, grayscale everything and crank the brightness.
            // Note: The 1.5x is a visual guess to make it look closer to vanilla.
            if (hasInvulnerability != 0)
            {
                float maxColor = max(max(fragColor.x, fragColor.y), fragColor.z);
                maxColor *= 1.5;
                fragColor.xyz = vec3(maxColor, maxColor, maxColor);
            }
        }
    ";
}
