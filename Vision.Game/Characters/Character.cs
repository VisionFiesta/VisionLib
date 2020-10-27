using System.Collections.Generic;
using System.Linq;
using Vision.Game.Characters.Shape;
using Vision.Game.Content.GameObjects;
using Vision.Game.Content.Items;
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
        public CharacterShape Shape { get; private set; }
        public CharacterState CharacterState { get; private set; }
        public Equipment Equipment { get; set; }

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

        public string MapIndx { get; set; }

        public bool QuestsNeedClear { get; private set; }
        public List<PlayerQuestInfo> DoingQuests { get; } = new List<PlayerQuestInfo>();
        public List<PlayerQuestDoneInfo> DoneQuests { get; } = new List<PlayerQuestDoneInfo>();

        public Character(ushort handle, Avatar avatar) : base(handle, GameObjectType.GOT_CHARACTER)
        {
            CharNo = avatar.CharNo;

            Equipment = avatar.Equipment;
            
            
            Name = avatar.Name;


            Slot = avatar.Slot;
        }

        public void Initialize(NcCharClientBaseCmd data)
        {
            CharNo = data.CharNo;
            Name = data.CharName;
            Level = data.Level;
            Stats.CurrentHP = data.HP;
            Stats.CurrentSP = data.SP;
            Stats.CurrentLP = data.LP;
            Stats.BonusSTR = data.STRBonus;
            Stats.BonusEND = data.ENDBonus;
            Stats.BonusDEX = data.DEXBonus;
            Stats.BonusINT = data.INTBonus;
            Stats.BonusSPR = data.SPRBonus;

            Slot = data.Slot;
            EXP = data.EXP;
            HPStones = data.HPStones;
            SPStones = data.SPStones;
            Fame = data.Fame;
            Money = data.Money;
            KillPoints = data.KillPoints;

            Position = data.Position;
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

        public void SetShape(CharacterShape shape) => Shape = shape;

        public void UpdateTitles(CharTitleInfo newCurrentTitle, IEnumerable<CharTitleInfo> titles = null)
        {
            CurrentTitle = newCurrentTitle;
            if (titles != null)
            {
                Titles = titles.ToArray();
            }
        }

        public void UpdateDoingQuests(bool needClear, IEnumerable<PlayerQuestInfo> quests)
        {
            QuestsNeedClear = needClear;
            DoingQuests.AddRange(quests);
        }

        public void UpdateDoneQuests(IEnumerable<PlayerQuestDoneInfo> quests) => DoneQuests.AddRange(quests);


        // public static Character FromAvatar(Avatar avatar)
        // {
        //     var character = new Character(avatar.CharNo)
        //     {
        //         Name = avatar.Name,
        //         Level = avatar.Level,
        //         Shape = avatar.Shape,
        //         Equipment = avatar.Equipment
        //     };
        //
        //     return character;
        // }

        public override string ToString()
        {
            return $"Character - Name: {Name}, Level: {Level}, Class: {Shape.Class}, Gender: {Shape.Gender} Mode: {CharacterState}, Handle: {Handle}";
        }
    }
}
