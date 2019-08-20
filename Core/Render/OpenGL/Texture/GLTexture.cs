using System;
using System.Numerics;
using Helion.Render.OpenGL.Context;
using Helion.Render.OpenGL.Context.Types;
using Helion.Util.Geometry;
using static Helion.Util.Assertion.Assert;

namespace Helion.Render.OpenGL.Texture
{
    public abstract class GLTexture : IDisposable
    {
        /// <summary>
        /// A unique identifier for this texture so we could look it up by this
        /// index.
        /// </summary>
        public readonly int Id;
        
        /// <summary>
        /// The OpenGL texture 'name' handle.
        /// </summary>
        public readonly int TextureId;
        
        /// <summary>
        /// A readable name for this texture.
        /// </summary>
        public readonly string Name;
        
        /// <summary>
        /// A precalculated inverse of the UV coordinates.
        /// </summary>
        public readonly Vector2 UVInverse;
        
        /// <summary>
        /// The dimension of this texture.
        /// </summary>
        public readonly Dimension Dimension;
        
        /// <summary>
        /// What type of texture it is with respect to OpenGL.
        /// </summary>
        public readonly TextureTargetType TextureType;
        
        protected readonly IGLFunctions gl;

        protected GLTexture(int id, int textureId, string name, Dimension dimension, IGLFunctions functions, 
            TextureTargetType textureType)
        {
            Id = id;
            TextureId = textureId;
            Name = name;
            Dimension = dimension;
            UVInverse = Vector2.One / dimension.ToVector().ToFloat();
            gl = functions;
            TextureType = textureType;
        }

        ~GLTexture()
        {
            Fail($"Did not dispose of {GetType().FullName}, finalizer run when it should not be");
            ReleaseUnmanagedResources();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        protected virtual void ReleaseUnmanagedResources()
        {
            gl.DeleteTexture(TextureId);
        }
    }
}