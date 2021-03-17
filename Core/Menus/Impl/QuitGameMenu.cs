﻿using System;
using Helion.Audio.Sounds;
using Helion.Input;
using Helion.Menus.Base;
using Helion.Menus.Base.Text;
using Helion.Resources.Archives.Collection;
using Helion.Util.Configs;
using Helion.Util.Consoles;
using Helion.Util.RandomGenerators;

namespace Helion.Menus.Impl
{
    public class QuitGameMenu : Menu
    {
        private readonly Func<Menu?> m_quitAction;
        
        public QuitGameMenu(Config config, HelionConsole console, SoundManager soundManager, ArchiveCollection archiveCollection) :
            base(config, console, soundManager, archiveCollection, 90)
        {
            m_quitAction = () =>
            {
                Console.SubmitInputText("exit");
                return null;
            };

            var quitMessages = archiveCollection.Definitions.MapInfoDefinition.GameDefinition.QuitMessages;
            if (quitMessages.Count > 0)
            {
                TrueRandom random = new TrueRandom();
                string msg = archiveCollection.Definitions.Language.GetDefaultMessage(quitMessages[random.NextByte() % quitMessages.Count]);
                string[] lines = msg.Split(new char[] { '\n' });
                foreach (string line in lines)
                {
                    Components = Components.Add(new MenuSmallTextComponent(line));
                    Components = Components.Add(new MenuPaddingComponent(8));
                }

                Components = Components.Add(new MenuSmallTextComponent(archiveCollection.Definitions.Language.GetDefaultMessage("$DOSY")));
            }
            else
            {
                m_quitAction();
            }

            SetToFirstActiveComponent();
        }

        public override void HandleInput(InputEvent input)
        {
            if (input.ConsumeKeyPressed(Key.Y))
                m_quitAction();
            
            base.HandleInput(input);
        }
    }
}
