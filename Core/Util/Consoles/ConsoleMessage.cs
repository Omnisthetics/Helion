﻿using Helion.Graphics.String;

namespace Helion.Util.Consoles
{
    public readonly struct ConsoleMessage
    {
        public readonly ColoredString Message;
        public readonly long TimeNanos;

        public ConsoleMessage(ColoredString message, long timeNanos)
        {
            Message = message;
            TimeNanos = timeNanos;
        }
    }
}