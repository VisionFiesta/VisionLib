﻿using System;
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

        public uint CharNo { get; }
        public string Name { get; private set; }
        public CharacterShape Shape { get; private set; }
        public CharacterState CharacterState { get; private set; }
        public Equipment Equipment { get; private set; }

        public CharTitleInfo CurrentTitle;
        public CharTitleInfo[] Titles { get; private set; }

        public CharacterParameters Parameters = new CharacterParameters();

        public byte[] WindowPosData { get; set; }
        public ShortCutData[] ShortcutData { get; set; }
        public byte[] ShortcutSizeData { get; set; }
        public byte[] GameOptionData { get; set; }
        public byte[] KeyMapData { get; set; }

        public byte Slot { get; set; }
        public ulong EXP { get; set; }
        public ushort HPStones { get; set; }
        public ushort SPStones { get; set; }
        public uint Fame { get; set; }
        public ulong Money { get; set; }
        public uint KillPoints { get; set; }
        // public byte FreeStat_Points { get; set; }
        // public int FriendPoints { get; set; }

        public bool QuestsNeedClear { get; private set; }
        public List<PlayerQuestInfo> DoingQuests { get; } = new List<PlayerQuestInfo>();
        public List<PlayerQuestDoneInfo> DoneQuests { get; } = new List<PlayerQuestDoneInfo>();

        public Character(uint charNo)
        {
            CharNo = charNo;
            Type = GameObjectType.GOT_CHARACTER;
        }

        public void Initialize(NcCharClientBaseCmd data)
        {
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

        public void Initialize(NcBriefInfoLoginCharacterCmd data)
        {
            Handle = data.Handle;
            Level = data.Level;
            Name = data.CharID;
            Position = data.Position;
            Shape = new CharacterShape(data.Shape);
            Shape.Class = data.Class;
            CharacterState = data.State;

            CurrentTitle = new CharTitleInfo
            {
                Type = data.CharTitle.Type,
                ElementNo = data.CharTitle.ElementNo,
                ElementValue = data.CharTitle.ElementValue
            };
        }

        public void SetShape(CharacterShape shape)
        {
            Shape = shape;
        }

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

        public void UpdateDoneQuests(IEnumerable<PlayerQuestDoneInfo> quests)
        {
            DoneQuests.AddRange(quests);
        }

        // TODO:
        public void UpdateChargedBuffs()
        {

        }

        public static Character FromAvatar(Avatar avatar)
        {
            var character = new Character(avatar.CharNo)
            {
                Name = avatar.Name,
                Level = avatar.Level,
                Shape = avatar.Shape,
                Equipment = avatar.Equipment
            };

            return character;
        }

        public override string ToString()
        {
            return $"Character - Name: {Name}, Level: {Level}, Class: {Shape.Class}, Gender: {Shape.Gender} Mode: {CharacterState}";
        }
    }
}
