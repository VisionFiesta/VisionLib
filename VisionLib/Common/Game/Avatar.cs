using System;
using VisionLib.Common.Game.Characters;
using VisionLib.Common.Game.Characters.Shape;
using VisionLib.Common.Game.Content.Items;
using VisionLib.Common.Networking.Structs.Common;

namespace VisionLib.Common.Game
{
    /// <summary>
    /// Class that contains avatar information. This class is not used
    /// in-game, but just stands to display a <see cref="Character"/> instance.
    /// </summary>
    public class Avatar
    {

        public int CharNo { get; set; }

        public DateTime DeleteTime { get; set; }

        public Equipment Equipment { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime KQDate { get; set; }

        public int KQHandle { get; set; }

        public string KQMapIndx { get; set; }

        public STRUCT_SHINE_XY_TYPE KQPosition { get; set; }

        public byte Level { get; set; }

        public string MapIndx { get; set; }

        public string Name { get; set; }

        public CharacterShape Shape { get; set; }

        public byte Slot { get; set; }

        public PROTO_TUTORIAL_INFO TutorialState { get; set; }

        public byte[] WindowPosData { get; set; }
        public byte[] ShortcutData { get; set; }
        public byte[] ShortcutSizeData { get; set; }
        public byte[] GameOptionData { get; set; }
        public byte[] KeyMapData { get; set; }

        public bool HasLoggedIn { get; set; }

        public Avatar(int charNo)
        {
            CharNo = charNo;
            Shape = new CharacterShape();
            Equipment = new Equipment();
        }

        public void SetShape(PROTO_AVATAR_SHAPE_INFO info)
        {
            Shape = new CharacterShape(info);
        }
    }
}
