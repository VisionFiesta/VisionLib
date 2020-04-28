﻿using System.Collections.Generic;
using VisionLib.Common.Extensions;
using VisionLib.Common.Game.Characters;

namespace VisionLib.Common.Game
{
    public class Account
    {
        public ushort AccountID;

        public readonly List<Avatar> Avatars = new List<Avatar>();

        public Avatar ActiveAvatar { get; private set; }
        public Character ActiveCharacter { get; private set; }

        public void SelectAvatar(uint charNo)
        {
            ActiveAvatar = Avatars.First(avatar => avatar.CharNo == charNo);
            ActiveAvatar.HasLoggedIn = true;
        }

        public void ChooseCharacter(uint charNo)
        {
            SelectAvatar(charNo);
            ActiveCharacter = Character.FromAvatar(ActiveAvatar);
        }
    }
}
