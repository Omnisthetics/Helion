﻿using System;
using Helion.Graphics.String;
using Helion.Util;
using static Helion.Util.Assertion.Assert;

namespace Helion.Menus.Base.Text
{
    /// <summary>
    /// A menu component that is made up of text.
    /// </summary>
    public abstract class MenuTextComponent : IMenuComponent
    {
        public readonly ColoredString Text;
        public readonly int Size;
        public readonly CIString FontName;
        public Func<Menu?>? Action { get; }

        public MenuTextComponent(ColoredString text, int size, CIString fontName, Func<Menu?>? action = null)
        {
            Precondition(size > 0, "Cannot have a zero or negative font size");
            
            Text = text;
            Size = size;
            FontName = fontName;
            Action = action;
        }

        public override string ToString() => Text.ToString();
    }
}