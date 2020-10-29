using System;
using System.Collections.Generic;
using System.Linq;
using Vision.Game.Characters.Shape;
using Vision.Game.Content.GameObjects;
using Vision.Game.Structs.Char;
using Vision.Game.Structs.Common;

namespace Vision.Game.Characters
{
    public class WorldCharacter
    {
        public uint CharNo { get; private set; }
        public string CharName { get; private set; }
        public ushort Level { get; private set; }
        public byte Slot { get; private set; }
        public string MapName { get; private set; }
        public ProtoAvatarDeleteInfo DeleteInfo { get; private set; }
        public CharacterShape Shape { get; private set; }
        public ProtoEquipment Equipment { get; private set; }
        public int KQHandle { get; private set; }
        public string KQMapName { get; private set; }
        public ShineXY KQPosition { get; private set; }
        public ShineDatetime KQDate { get; private set; }
        public CharIdChangeData CharIdChangeData { get; private set; }
        public ProtoTutorialInfo TutorialInfo { get; private set; }

        // Sequential update data
        public NcCharClientBaseCmd BaseData { get; private set; }
        public NcCharClientQuestDoingCmd QuestDoingData { get; set; }
        public NcCharClientQuestDoneCmd QuestDoneData { get; set; }
        public NcCharClientCharTitleCmd TitleData { get; set; }


        public void Update(NcCharClientBaseCmd data) => BaseData = data;

        public void Update(NcCharClientShapeCmd data) => Shape = data.Shape;

        public void Update(NcCharClientQuestDoingCmd data) => QuestDoingData = data;

        public void Update(NcCharClientQuestDoneCmd data) => QuestDoneData = data;

        public void Update(NcCharClientCharTitleCmd data) => TitleData = data;

        public static WorldCharacter FromAvatarInformation(ProtoAvatarInformation avatar)
        {
            return new WorldCharacter()
            {
                CharNo = avatar.CharNo,
                CharName = avatar.CharName,
                Level = avatar.Level,
                Slot = avatar.CharSlot,
                MapName = avatar.LoginMap,
                DeleteInfo = avatar.DeleteInfo,
                Shape = new CharacterShape(avatar.CharShape),
                Equipment = avatar.Equipment,
                KQHandle = avatar.KQHandle,
                KQMapName = avatar.KQMapName,
                KQPosition = avatar.nKQCoord,
                KQDate = avatar.KQDate,
                CharIdChangeData = avatar.CharIDChangeData,
                TutorialInfo = avatar.TutorialInfo
            };
        }
    }
}
