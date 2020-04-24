using System.Collections.Generic;
using VisionLib.Common.Extensions;

namespace VisionLib.Common.Game
{
    public class Account
    {
        public readonly List<Avatar> Avatars = new List<Avatar>();

        public Avatar ActiveAvatar { get; private set; }

        public void SelectAvatar(byte slot)
        {
            ActiveAvatar = Avatars.First(a => a.Slot == slot);
        }
    }
}
