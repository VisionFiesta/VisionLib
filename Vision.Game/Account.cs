using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Vision.Core.Logging.Loggers;
using Vision.Game.Characters;

namespace Vision.Game
{
    public class Account
    {
        private static readonly EngineLog Logger = new EngineLog(typeof(Account));

        public ushort AccountID;

        public string AccountName;

        public readonly List<Avatar> Avatars = new List<Avatar>();
        private readonly Dictionary<uint, Character> _characters = new Dictionary<uint, Character>();

        public Avatar ActiveAvatar { get; private set; }
        public Character ActiveCharacter { get; private set; }

        private void SelectAvatar(uint charNo)
        {
            ActiveAvatar = Avatars.First(avatar => avatar.CharNo == charNo);
            ActiveAvatar.HasLoggedIn = true;
        }

        public void ChooseCharacter(uint charNo)
        {
            SelectAvatar(charNo);

            if (!_characters.ContainsKey(charNo))
            {
                _characters.Add(charNo, Character.FromAvatar(ActiveAvatar));
            }

            if (_characters.TryGetValue(charNo, out var character))
            {
                ActiveCharacter = character;
            }
            else
            {
                Logger.Error("Failed to get character!");
            }


        }
    }
}
