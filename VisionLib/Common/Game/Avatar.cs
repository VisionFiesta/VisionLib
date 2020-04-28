using System;
using VisionLib.Common.Game.Characters;
using VisionLib.Common.Game.Characters.Shape;
using VisionLib.Common.Game.Content.Items;
using VisionLib.Core.Struct.Common;

namespace VisionLib.Common.Game
{
    /// <summary>
    /// Class that contains avatar information. This class is not used
    /// in-game, but just stands to display a <see cref="Character"/> instance.
    /// </summary>
    public class Avatar
    {
        public uint CharNo { get; set; }

        public DateTime DeleteTime { get; set; }

        public Equipment Equipment { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime KQDate { get; set; }

        public int KQHandle { get; set; }

        public string KQMapIndx { get; set; }

        public ShineXY KQPosition { get; set; }

        public byte Level { get; set; }

        public string MapIndx { get; set; }

        public string Name { get; set; }

        public CharacterShape Shape { get; set; }

        public byte Slot { get; set; }

        public ProtoTutorialInfo TutorialState { get; set; }

        public bool HasLoggedIn { get; set; }

        public Avatar(uint charNo)
        {
            CharNo = charNo;
            Shape = new CharacterShape();
            Equipment = new Equipment();
        }

        public void SetShape(ProtoAvatarShapeInfo info)
        {
            Shape = new CharacterShape(info);
        }
    }
}
