﻿using System;
using System.Linq;

namespace Helion.World.Cheats
{
    public class MultiCodeCheat : ICheat
    {
        private readonly string[] m_codes;

        public string CheatName { get; }
        public string? ConsoleCommand { get; }
        public CheatType CheatType { get; }
        public bool Activated { get; set; }
        public bool IsToggleCheat { get; private set; }
        public bool ClearTypedCheatString { get; private set; }

        public MultiCodeCheat(string name, string[] codes, string consoleCommand, CheatType cheatType, bool canToggle = true,
            bool clearTypedCheatString = true)
        {
            CheatName = name;
            ConsoleCommand = consoleCommand;
            m_codes = codes;
            CheatType = cheatType;
            IsToggleCheat = canToggle;
            ClearTypedCheatString = clearTypedCheatString;
        }

        public bool IsMatch(string str) => m_codes.Any(x => x.Equals(str, StringComparison.InvariantCultureIgnoreCase));

        public bool PartialMatch(string str) => m_codes.Any(x => x.StartsWith(str, StringComparison.InvariantCultureIgnoreCase));
    }
}