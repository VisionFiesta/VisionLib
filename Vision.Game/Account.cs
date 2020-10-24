using System.Collections.Generic;
using System.Linq;
using Vision.Game.Characters;

namespace Vision.Game
{
    public class Account
    {
        public ushort AccountID;

        public string AccountName;

        public readonly List<Avatar> Avatars = new List<Avatar>();

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
            ActiveCharacter = Character.FromAvatar(ActiveAvatar);
        }
    }
}
