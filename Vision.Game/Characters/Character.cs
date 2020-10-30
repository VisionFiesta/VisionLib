using System.Collections.Generic;
using System.Linq;
using Vision.Game.Characters.Shape;
using Vision.Game.Content.GameObjects;
using Vision.Game.Structs.BriefInfo;
using Vision.Game.Structs.Char;
using Vision.Game.Structs.Common;

namespace Vision.Game.Characters
{
    public class Character : GameObject
    {
        public static CharacterCommon Global { get; set; }

        public uint CharNo { get; private set; }
        public string Name { get; private set; }
        public CharacterShape Shape { get; set; }
        public CharacterState CharacterState { get; set; }
        public ProtoEquipment Equipment { get; set; }

        public CharTitleInfo CurrentTitle;
        public CharTitleInfo[] Titles { get; private set; }

        public CharParameterData Parameters = new CharParameterData();

        // public byte[] WindowPosData { get; private set; }
        public ShortCutData[] ShortcutData { get; set; }
        // public byte[] ShortcutSizeData { get; private set; }
        // public byte[] GameOptionData { get; private set; }
        // public byte[] KeyMapData { get; set; }

        public byte Slot { get; private set; }
        public ulong EXP { get; private set; }
        public ushort HPStones { get; private set; }
        public ushort SPStones { get; private set; }
        public uint Fame { get; private set; }
        public ulong Money { get; private set; }
        public uint KillPoints { get; private set; }
        // public byte FreeStat_Points { get; set; }
        public int FriendPoints { get; set; }
        
        public bool QuestsNeedClear { get; private set; }
        public List<PlayerQuestInfo> DoingQuests { get; private set; } = new List<PlayerQuestInfo>();
        public List<PlayerQuestDoneInfo> DoneQuests { get; private set; } = new List<PlayerQuestDoneInfo>();

        // various random stuff that gets sent
        public ProtoAvatarDeleteInfo DeleteInfo { get; private set; }
        public int KQHandle { get; private set; }
        public string KQMapName { get; private set; }
        public ShineXY KQPosition { get; private set; }
        public CharIdChangeData CharIdChangeData { get; private set; }
        public ProtoTutorialInfo TutorialInfo { get; private set; }

        public Character(ushort handle, WorldCharacter avatar) : base(handle, GameObjectType.GOT_CHARACTER)
        {
            CharNo = avatar.CharNo;
            Name = avatar.CharName;
            Slot = avatar.Slot;
            MapName = avatar.MapName;
            Shape = avatar.Shape;
            Equipment = avatar.Equipment;

            // the lame stuff
            DeleteInfo = avatar.DeleteInfo;
            KQHandle = avatar.KQHandle;
            KQMapName = avatar.KQMapName;
            KQPosition = avatar.KQPosition;
            CharIdChangeData = avatar.CharIdChangeData;
            TutorialInfo = avatar.TutorialInfo;

            // Base data
            var baseData = avatar.BaseData;
            Stats.CurrentHP = baseData.HP;
            Stats.CurrentSP = baseData.SP;
            Stats.CurrentLP = baseData.LP;
            Stats.BonusSTR = baseData.STRBonus;
            Stats.BonusEND = baseData.ENDBonus;
            Stats.BonusDEX = baseData.DEXBonus;
            Stats.BonusINT = baseData.INTBonus;
            Stats.BonusSPR = baseData.SPRBonus;

            EXP = baseData.EXP;
            HPStones = baseData.HPStones;
            SPStones = baseData.SPStones;
            Fame = baseData.Fame;
            Money = baseData.Money;
            KillPoints = baseData.KillPoints;

            Position = baseData.Position;

            // Shape data
            Shape = avatar.Shape;

            // QuestDoing data
            var questDoingData = avatar.QuestDoingData;
            QuestsNeedClear = questDoingData.NeedClear;
            DoingQuests = new List<PlayerQuestInfo>(questDoingData.QuestDoingArray);

            // QuestDone data
            var questDoneData = avatar.QuestDoneData;
            DoneQuests = new List<PlayerQuestDoneInfo>(questDoneData.QuestDoneArray);

            // Title data
            var titleData = avatar.TitleData;
            CurrentTitle = titleData.CurrentTitle;
            Titles = titleData.TitleArray;
        }

        public Character(NcBriefInfoLoginCharacterCmd data) : base(data.Handle, GameObjectType.GOT_CHARACTER)
        {
            Level = data.Level;
            Name = data.CharID;
            Position = data.Position;
            Shape = new CharacterShape(data.Shape) { Class = data.Class };
            CharacterState = data.State;

            CurrentTitle = new CharTitleInfo
            {
                Type = data.CharTitle.Type,
                ElementNo = data.CharTitle.ElementNo,
                ElementValue = data.CharTitle.ElementValue
            };
        }

        public override string ToString() => $"Character - Name: {Name}, Level: {Level}, Class: {Shape.Class}, Gender: {Shape.Gender} Mode: {CharacterState}, Handle: {Handle}";
    }
}
