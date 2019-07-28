﻿using System;
using System.Collections.Generic;
using System.Linq;
using Helion.Input;


namespace Helion.Cheats
{
    public class CheatManager
    {
        private readonly ICheat[] m_cheats = new ICheat[]
        {
            new ExactMatchCheat("No clipping mode", "idclip", "noclip", CheatType.NoClip),
            new ChangeLevelCheat(),
        };

        private readonly Dictionary<CheatType, ICheat> m_cheatLookup = new Dictionary<CheatType, ICheat>();
        private string m_currentCheat = string.Empty;

        public event EventHandler<ICheat> CheatActivationChanged;

        public static CheatManager Instance { get; } = new CheatManager();

        public CheatManager()
        {
            m_cheatLookup = m_cheats.ToDictionary(x => x.CheatType);
        }

        public void ActivateCheat(CheatType cheatType)
        {
            if (m_cheatLookup.ContainsKey(cheatType))
            {
                var cheat = m_cheatLookup[cheatType];

                if (cheat.IsToggleCheat)
                    cheat.Activated = !cheat.Activated;

                CheatActivationChanged?.Invoke(this, cheat);
            }
        }

        public void HandleInput(ConsumableInput consumableInput)
        {
            var keys = consumableInput.GetTypedCharacters();

            foreach (var key in keys)
            {
                m_currentCheat += key.ToString().ToLower();

                if (m_cheats.Any(x => x.PartialMatch(m_currentCheat)))
                {
                    var cheat = m_cheats.FirstOrDefault(x => x.IsMatch(m_currentCheat));
                    if (cheat != null)
                    {
                        ActivateCheat(cheat.CheatType);
                        m_currentCheat = string.Empty;
                    }
                }
                else
                {
                    m_currentCheat = string.Empty;
                }
            }
        }

        public bool IsCheatActive(CheatType cheat)
        {
            if (m_cheatLookup.ContainsKey(cheat))
                return m_cheatLookup[cheat].Activated;
            return false;
        }
    }
}
