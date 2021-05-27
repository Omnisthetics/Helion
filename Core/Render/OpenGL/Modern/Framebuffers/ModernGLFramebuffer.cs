﻿using System;
using Helion.Geometry;
using Helion.Render.Common.Framebuffer;
using Helion.Render.OpenGL.Modern.Renderers.Hud;
using Helion.Render.OpenGL.Modern.Renderers.World;
using Helion.Render.OpenGL.Modern.Textures;
using Helion.Render.OpenGL.Textures;
using OpenTK.Graphics.OpenGL4;
using static Helion.Util.Assertion.Assert;

namespace Helion.Render.OpenGL.Modern.Framebuffers
{
    public abstract class ModernGLFramebuffer : IFramebuffer
    {
        protected const int DefaultFramebufferName = 0;
        
        public string Name { get; }
        public abstract Dimension Dimension { get; }
        public abstract GLTexture Texture { get; }
        private readonly ModernGLHudRenderer m_hudRenderer;
        private readonly ModernGLWorldRenderer m_worldRenderer;
        private bool m_disposed;
        
        protected ModernGLFramebuffer(string name, ModernGLTextureManager textureManager)
        {
            Name = name;
            m_hudRenderer = new ModernGLHudRenderer(this, textureManager);
            m_worldRenderer = new ModernGLWorldRenderer(textureManager);
        }

        ~ModernGLFramebuffer()
        {
            FailedToDispose(this);
            PerformDispose();
        }

        public abstract void Bind();
        public abstract void Unbind();
        
        public void BindAnd(Action action)
        {
            Bind();
            action();
            Unbind();
        }
        
        public void Render(Action<FramebufferRenderContext> action)
        {
            Bind();
            
            (int w, int h) = Dimension;
            GL.Viewport(0, 0, w, h);
            GL.Scissor(0, 0, w, h);
            
            FramebufferRenderContext ctx = new(this, m_hudRenderer, m_worldRenderer);
            action(ctx);
            
            Unbind();
        }

        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
            PerformDispose();
        }

        private void PerformDispose()
        {
            if (m_disposed)
                return;

            m_hudRenderer.Dispose();
            m_worldRenderer.Dispose();

            m_disposed = true;
        }
    }
}