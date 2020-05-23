using System.Collections.Generic;

namespace Vision.Core.IO.SHN
{
    public enum SHNType
    {
        AbState,
        AbStateMsg,
        AbStateSaveTypeInfo,
        AbStateView,
        AccUpgrade,
        AccUpGradeInfo,
        ActionEffectAbState,
        ActionEffectInfo,
        ActionEffectItem,
        ActionRangeFactor,
        ActionViewInfo,
        ActiveSkill,
        ActiveSkillGroup,
        ActiveSkillInfoServer,
        ActiveSkillView,
        AdminLvSet,
        AnimationEffectInfo,
        AnimationSoundInfo,
        AnimationViewInfo,
        AnnounceData,
        AreaSkill,
        AttendReward,
        AttendSchedule,
        BadNameFilter,
        BasicInfoFind,
        BasicInfoFindUI,
        BasicInfoFindWord,
        BasicInfoHelp,
        BasicInfoLink,
        BasicInfoTip,
        BasicInfoTipCycle,
        BasicInfoTitle,
        BelongDice,
        BelongTypeInfo,
        BMP,
        BRAccUpgrade,
        BRAccUpgradeInfo,
        CharacterTitleData,
        CharacterTitleStateServer,
        CharacterTitleStateView,
        ChargedDeletableBuff,
        ChargedEffect,
        ChargedIconItem,
        ChargedMessageItem,
        ChatColor,
        ChrBasicEquip,
        ChrCreateEquip,
        ClassName,
        CollectCard,
        CollectCardDropRate,
        CollectCardGroupDesc,
        CollectCardMobGroup,
        CollectCardReward,
        CollectCardStarRate,
        CollectCardTitle,
        CollectCardView,
        ColorInfo,
        DamageEffect,
        DamageLvGapEVP,
        DamageLvGapPVE,
        DamageLvGapPVP,
        DamageSoundInfo,
        DiceDividind,
        DiceGame,
        DiceRate,
        EffectViewInfo,
        EmotionFilter,
        EnchantSocketRate,
        ErrorCodeTable,
        FaceCutMsg,
        FaceInfo,
        FieldLvCondition,
        FontSet,
        FriendPointReward,
        Gather,
        GBBanTime,
        GBDiceDividind,
        GBDiceGame,
        GBDiceRate,
        GBEventCode,
        GBExchangeMaxCoin,
        GBHouse,
        GBJoinGameMember,
        GBReward,
        GBSMAll,
        GBSMBetCoin,
        GBSMCardRate,
        GBSMCenter,
        GBSMGroup,
        GBSMJPRate,
        GBSMLine,
        GBSMNPC,
        GBTaxRate,
        GradeItemOption,
        GroupAbState,
        GTIBreedSubject,
        GTIGetRate,
        GTIGetRateGap,
        GTIServer,
        GTIView,
        GTWinScore,
        GuildAcademy,
        GuildAcademyLevelUp,
        GuildAcademyRank,
        GuildGradeData,
        GuildGradeScoreData,
        GuildLevelScoreData,
        GuildTournament,
        GuildTournamentLvGap,
        GuildTournamentMasterBuff,
        GuildTournamentOccupy,
        GuildTournamentRequire,
        GuildTournamentReward,
        GuildTournamentScore,
        GuildTournamentSkill,
        GuildTournamentSkillDesc,
        HairColorInfo,
        HairInfo,
        HolyPromiseReward,
        ItemAction,
        ItemActionCondition,
        ItemActionEffect,
        ItemActionEffectDesc,
        ItemDismantle,
        ItemDropLog,
        ItemInfo,
        ItemInfoServer,
        ItemInvenDel,
        ItemMerchantInfo,
        ItemMix,
        ItemMoney,
        ItemOptions,
        ItemPackage,
        ItemServerEquipTypeInfo,
        ItemShop,
        ItemShopView,
        ItemSort,
        ItemUpgrade,
        ItemUseEffect,
        ItemViewDummy,
        ItemViewEquipTypeInfo,
        ItemViewInfo,
        JobEquipInfo,
        KingdomQuest,
        KingdomQuestDesc,
        KingdomQuestMap,
        KingdomQuestRew,
        KQIsVote,
        KQItem,
        KQTeam,
        KQVoteDesc,
        KQVoteMajorityRate,
        LCGroupRate,
        LCReward,
        MapBuff,
        MapInfo,
        MapInfoV,
        MapLinkPoint,
        MapViewInfo,
        MapWayPoint,
        MarketSearchInfo,
        MHEmotion,
        MiniHouse,
        MiniHouseDummy,
        MiniHouseEndure,
        MiniHouseFurniture,
        MiniHouseFurnitureObjEffect,
        MiniHouseObjAni,
        MinimonAutoUseItem,
        MinimonInfo,
        MobAbStateDropSetting,
        MobAutoAction,
        MobConditionServer,
        MobConditionView,
        MobCoordinate,
        MobInfo,
        MobInfoServer,
        MobKillAble,
        MobKillAnnounce,
        MobKillAnnounceText,
        MobKillLog,
        MobLifeTime,
        MobNoFadeIn,
        MobRandomIdleAni,
        MobRegenAni,
        MobResist,
        MobSpecies,
        MobViewInfo,
        MobWeapon,
        MoverAbility,
        MoverHG,
        MoverItem,
        MoverMain,
        MoverSlotCharAni,
        MoverSlotView,
        MoverUpgradeEffect,
        MoverUseSkill,
        MoverView,
        MsgWorldManager,
        MultiHitType,
        MysteryVaultServer,
        NpcDialogData,
        NpcSchedule,
        NPCViewInfo,
        PartyBonusByLvDiff,
        PartyBonusByMember,
        PartyBonusLimit,
        PassiveSkill,
        PassiveSkillView,
        Produce,
        ProduceView,
        PSkillSetAbstate,
        PupCase,
        PupCaseDesc,
        PupFactorCondition,
        PupMain,
        PupMind,
        PupPriority,
        PupServer,
        PupView,
        QuestData,
        QuestDialog,
        QuestReward,
        QuestScript,
        QuestSpecies,
        RaceNameInfo,
        RandomOption,
        RandomOptionCount,
        RareMoverEachRate,
        RareMoverRate,
        RareMoverSubRate,
        ReactionType,
        Riding,
        ScriptMsg,
        SetEffect,
        SetItem,
        SetItemEffect,
        SetItemName,
        SetItemView,
        ShineReward,
        SingleData,
        SlanderFilter,
        SpamerPenalty,
        SpamerPenaltyRule,
        SpamerReport,
        StateField,
        StateItem,
        StateMob,
        SubAbState,
        TermExtendMatch,
        TermExtendMatchGroupDesc,
        TextData,
        TextData2,
        TextData3,
        ToggleSkill,
        TownPortal,
        UpEffect,
        UpgradeInfo,
        UseClassTypeInfo,
        WeaponAttrib,
        WeaponTitleData,
        WorldMapAvatarInfo,
        WPConfig,
        Unknown,
        DeprecatedFiles,
        QuestEndNpc,
        QuestEndItem,
        QuestAction
    }

    public static class SHNTypeExtensions
    {
        public static readonly List<SHNType> EUHashOrder = new List<SHNType>()
        {
            SHNType.AbState,
            SHNType.ActiveSkill,
            SHNType.CharacterTitleData,
            SHNType.ChargedEffect,
            SHNType.ClassName,
            SHNType.Gather,
            SHNType.GradeItemOption,
            SHNType.ItemDismantle,
            SHNType.ItemInfo,
            SHNType.MapInfo,
            SHNType.MiniHouseFurniture,
            SHNType.MiniHouse,
            SHNType.MiniHouseObjAni,
            SHNType.MobInfo,
            SHNType.PassiveSkill,
            SHNType.Riding,
            SHNType.SubAbState,
            SHNType.UpgradeInfo,
            SHNType.WeaponAttrib,
            SHNType.WeaponTitleData,
            SHNType.MiniHouseFurnitureObjEffect,
            SHNType.MiniHouseEndure,
            SHNType.DiceDividind,
            SHNType.ActionViewInfo,
            SHNType.MapLinkPoint,
            SHNType.MapWayPoint,
            SHNType.AbStateView,
            SHNType.ActiveSkillView,
            SHNType.CharacterTitleStateView,
            SHNType.EffectViewInfo,
            SHNType.ItemShopView,
            SHNType.ItemViewInfo,
            SHNType.MapViewInfo,
            SHNType.MobViewInfo,
            SHNType.NPCViewInfo,
            SHNType.PassiveSkillView,
            SHNType.ProduceView,
            SHNType.CollectCardView,
            SHNType.GTIView,
            SHNType.ItemViewEquipTypeInfo,
            SHNType.SingleData,
            SHNType.MarketSearchInfo,
            SHNType.ItemMoney,
            SHNType.PupMain,
            SHNType.ChatColor,
            SHNType.TermExtendMatch,
            SHNType.MinimonInfo,
            SHNType.MinimonAutoUseItem,
            SHNType.ChargedDeletableBuff,
            SHNType.SlanderFilter,
        };

        public static readonly List<SHNType> NAHashOrder = new List<SHNType>(EUHashOrder)
        {
            SHNType.DeprecatedFiles,
            SHNType.QuestData,
            SHNType.QuestEndNpc,
            SHNType.QuestEndItem,
            SHNType.QuestAction,
            SHNType.QuestReward,
        };

        public static string ToFilename(this SHNType type) => $"{type}.shn";

        /// <summary>
        /// If the SHN is larger than 100kB
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsLarge(this SHNType type)
        {
            switch (type)
            {
                case SHNType.ItemViewInfo:
                case SHNType.ItemInfo:
                case SHNType.ItemInfoServer:
                case SHNType.QuestDialog:
                case SHNType.ActiveSkillView:
                case SHNType.ActiveSkill:
                case SHNType.AnimationEffectInfo:
                case SHNType.MobViewInfo:
                case SHNType.AnimationSoundInfo:
                case SHNType.UpgradeInfo:
                case SHNType.RandomOption:
                case SHNType.AnimationViewInfo:
                case SHNType.QuestSpecies:
                case SHNType.MobInfoServer:
                case SHNType.AbStateView:
                case SHNType.ItemViewDummy:
                case SHNType.MobSpecies:
                case SHNType.ActionEffectInfo:
                case SHNType.NPCViewInfo:
                case SHNType.MobInfo:
                case SHNType.GradeItemOption:
                case SHNType.ScriptMsg:
                case SHNType.ItemActionEffectDesc:
                case SHNType.MapLinkPoint:
                case SHNType.AbState:
                case SHNType.ChargedEffect:
                case SHNType.CharacterTitleStateView:
                case SHNType.ItemShop:
                case SHNType.MapWayPoint:
                case SHNType.TextData:
                case SHNType.SubAbState:
                case SHNType.PassiveSkillView:
                case SHNType.TextData2:
                case SHNType.MobCoordinate:
                case SHNType.WeaponTitleData:
                case SHNType.MoverView:
                case SHNType.PassiveSkill:
                case SHNType.CollectCardView:
                case SHNType.QuestReward:
                case SHNType.ItemShopView:
                case SHNType.BasicInfoLink:
                case SHNType.Produce:
                    return true;
                default:
                    return false;
            }
        }


        public static bool InHash_EU(this SHNType type)
        {
            switch (type)
            {
                case SHNType.AbState:
                case SHNType.ActiveSkill:
                case SHNType.CharacterTitleData:
                case SHNType.ChargedEffect:
                case SHNType.ClassName:
                case SHNType.Gather:
                case SHNType.GradeItemOption:
                case SHNType.ItemDismantle:
                case SHNType.ItemInfo:
                case SHNType.MapInfo:
                case SHNType.MiniHouseFurniture:
                case SHNType.MiniHouse:
                case SHNType.MiniHouseObjAni:
                case SHNType.MobInfo:
                case SHNType.PassiveSkill:
                case SHNType.Riding:
                case SHNType.SubAbState:
                case SHNType.UpgradeInfo:
                case SHNType.WeaponAttrib:
                case SHNType.WeaponTitleData:
                case SHNType.MiniHouseFurnitureObjEffect:
                case SHNType.MiniHouseEndure:
                case SHNType.DiceDividind:
                case SHNType.ActionViewInfo:
                case SHNType.MapLinkPoint:
                case SHNType.MapWayPoint:
                case SHNType.AbStateView:
                case SHNType.ActiveSkillView:
                case SHNType.CharacterTitleStateView:
                case SHNType.EffectViewInfo:
                case SHNType.ItemShopView:
                case SHNType.ItemViewInfo:
                case SHNType.MapViewInfo:
                case SHNType.MobViewInfo:
                case SHNType.NPCViewInfo:
                case SHNType.PassiveSkillView:
                case SHNType.ProduceView:
                case SHNType.CollectCardView:
                case SHNType.GTIView:
                case SHNType.ItemViewEquipTypeInfo:
                case SHNType.SingleData:
                case SHNType.MarketSearchInfo:
                case SHNType.ItemMoney:
                case SHNType.PupMain:
                case SHNType.ChatColor:
                case SHNType.TermExtendMatch:
                case SHNType.MinimonInfo:
                case SHNType.MinimonAutoUseItem:
                case SHNType.ChargedDeletableBuff:
                case SHNType.SlanderFilter:
                    return true;
                default: return false;
            }
        }
        public static bool InHash_NA(this SHNType type)
        {
            switch (type)
            {
                case SHNType.DeprecatedFiles:
                case SHNType.QuestData:
                case SHNType.QuestEndNpc:
                case SHNType.QuestEndItem:
                case SHNType.QuestAction:
                case SHNType.QuestReward:
                    return true;
                default:
                    return InHash_EU(type);
            }
        }
    }
}
