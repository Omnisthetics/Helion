﻿using System;
using System.Drawing;
using Helion.Geometry;
using Helion.Geometry.Vectors;
using Helion.Graphics.String;
using Helion.Render.Common;
using Helion.Render.Common.Enums;
using Helion.Render.Common.Renderers;
using Helion.Util.Consoles;
using Helion.Util.Timing;

namespace Helion.Layer.Consoles
{
    public partial class ConsoleLayer
    {
        private const int FontSize = 32;
        private const string FontName = "Console";
        private const long FlashSpanNanos = 500 * 1000L * 1000L;
        private const long HalfFlashSpanNanos = FlashSpanNanos / 2;
        
        private static bool IsCursorFlashTime => Ticker.NanoTime() % FlashSpanNanos < HalfFlashSpanNanos;
        
        public void Render(IRenderableSurfaceContext ctx, IHudRenderContext hud)
        {
            ctx.ClearDepth();

            RenderBackground(hud);
            RenderInput(hud, out int inputHeight);
            RenderMessages(hud, inputHeight);
        }

        private void RenderBackground(IHudRenderContext hud)
        {
            RenderConsoleBackground(hud);
            RenderConsoleDivider(hud);
        }

        private void RenderConsoleBackground(IHudRenderContext hud)
        {
            const string ConsoleBackingImage = "CONBACK";
            const string TitlepicImage = "TITLEPIC";
            const float BackgroundAlpha = 0.65f;

            int halfHeight = hud.Height / 2;
            HudBox drawArea = (0, -halfHeight, hud.Width, halfHeight);

            if (hud.Textures.HasImage(ConsoleBackingImage))
            {
                hud.Image(ConsoleBackingImage, drawArea);
                hud.FillBox(drawArea, Color.Black, alpha: BackgroundAlpha);
            }
            else if (hud.Textures.HasImage(TitlepicImage))
            {
                hud.Image(TitlepicImage, drawArea);
                hud.FillBox(drawArea, Color.Black, alpha: BackgroundAlpha);
            }
            else
            {
                hud.FillBox((0, 0, hud.Width, hud.Height), Color.Gray);
            }
        }

        private void RenderConsoleDivider(IHudRenderContext hud)
        {
            const int DividerHeight = 3;

            HudBox dividerArea = (0, 0, hud.Width, DividerHeight);
            hud.FillBox(dividerArea, Color.Black, Align.MiddleLeft);
        }

        private void RenderInput(IHudRenderContext hud, out int inputHeight)
        {
            hud.Text(m_console.Input, FontName, FontSize, (4, -4), out Dimension drawArea, 
                window: Align.MiddleLeft, anchor: Align.BottomLeft, color: Color.Yellow);

            if (drawArea.Height == 0)
                drawArea.Height = FontSize;
            
            RenderInputCursorFlash(hud, drawArea);

            inputHeight = drawArea.Height;
        }

        private void RenderInputCursorFlash(IHudRenderContext hud, Dimension drawArea)
        {
            const int CaretWidth = 2;

            if (!IsCursorFlashTime)
                return;

            int offsetX = m_console.Input == "" ? 4 : 6;
            Vec2I origin = (drawArea.Width + offsetX, -4);
            Vec2I dimension = (CaretWidth, drawArea.Height);
            
            HudBox area = (origin, origin + dimension);
            hud.FillBox(area, Color.LawnGreen, Align.MiddleLeft, Align.BottomLeft);
        }

        private void RenderMessages(IHudRenderContext hud, int inputHeight)
        {
            const int InputToMessagePadding = 8;
            const int BetweenMessagePadding = 7;

            int bottomY = (hud.Height / 2) - inputHeight - InputToMessagePadding;
            
            foreach (ConsoleMessage message in m_console.Messages)
            {
                if (bottomY <= 0)
                    break;

                int offsetX = 4;
                int maxDrawHeight = 0;
                
                foreach (ColoredChar coloredChar in message.Message)
                {
                    hud.Text(coloredChar.Char.ToString(), FontName, FontSize, (offsetX, bottomY), 
                        out Dimension drawArea, anchor: Align.BottomLeft, color: coloredChar.Color);
                    
                    offsetX += drawArea.Width;
                    maxDrawHeight = Math.Max(maxDrawHeight, drawArea.Height);

                    if (offsetX > hud.Width)
                        break;
                }

                bottomY -= maxDrawHeight + BetweenMessagePadding;
            }
        }
    }
}