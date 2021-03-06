﻿namespace Vision.Game.Enums
{
    public enum CharacterTitleType : byte
    {
        CTT_ID_NONE = 0x0,
        CTT_ID_EXP = 0x1,
        CTT_ID_CLASS = 0x2,
        CTT_ID_LOGON_TIME_AT_ONCE = 0x3,
        CTT_ID_PLAY_TIME = 0x4,
        CTT_ID_MONEY = 0x5,
        CTT_ID_MONEY_GIVE = 0x6,
        CTT_ID_MONEY_BEG = 0x7,
        CTT_ID_KILLED_BY_MOB = 0x8,
        CTT_ID_NOKILLED_BY_MOB = 0x9,
        CTT_ID_WEAPON_TITLE = 0x0A,
        CTT_ID_TOTAL_KILL_MOB = 0x0B,
        CTT_ID_KILL_GUILD = 0x0C,
        CTT_ID_KILLED_BY_GUILD = 0x0D,
        CTT_ID_TOTAL_GUILD_ATC = 0x0E,
        CTT_ID_MY_GUILD_GRADE = 0x0F,
        CTT_ID_FULL_PARTY_JOIN = 0x10,
        CTT_ID_ARENA_MY_WIN = 0x11,
        CTT_ID_ARENA_MY_LOST = 0x12,
        CTT_ID_ARENA_MY_ATT = 0x13,
        CTT_ID_KQ_ATT = 0x14,
        CTT_ID_KQ_SUCCESS = 0x15,
        CTT_ID_KQ_FAIL = 0x16,
        CTT_ID_SELL_NPC_COUNT = 0x17,
        CTT_ID_BUY_NPC_COUNT = 0x18,
        CTT_ID_NOJUM_SELL = 0x19,
        CTT_ID_AUCTION_SELL = 0x1A,
        CTT_ID_ITEM_UP_SUCCESS_COUNT = 0x1B,
        CTT_ID_ITEM_UP_FAIL_COUNT = 0x1C,
        CTT_ID_ITEM_UP_SUC_CON = 0x1D,
        CTT_ID_ITEM_UP_FAIL_CON = 0x1E,
        CTT_ID_ITEM_UP_MY_BEST = 0x1F,
        CTT_ID_ITEM_UP_DESTORY_COUNT = 0x20,
        CTT_ID_MASTER_PUPIL_COUNT = 0x21,
        CTT_ID_FRIEND_COUNT = 0x22,
        CTT_ID_MINI_FUNI_COUNT = 0x23,
        CTT_ID_MINI_VISITOR_COUNT = 0x24,
        CTT_ID_PET_BUY = 0x25,
        CTT_ID_PET_KILLED = 0x26,
        CTT_ID_RARE_ITEM_GET = 0x27,
        CTT_ID_QUEST_SUC_COUNT = 0x28,
        CTT_ID_NOCHATTING_COUNT = 0x29,
        CTT_ID_BROADCAST_COUNT = 0x2A,
        CTT_ID_CHAR_TITLE = 0x2B,
        CTT_ID_FAME_COUNT = 0x2C,
        CTT_ID_JOB_FIGHTER = 0x2D,
        CTT_ID_JOB_CLERIC = 0x2E,
        CTT_ID_JOB_ARCHER = 0x2F,
        CTT_ID_JOB_MAGE = 0x30,
        CTT_ID_PRODUCT_HP = 0x31,
        CTT_ID_PRODUCT_SP = 0x32,
        CTT_ID_PRODUCT_GS = 0x33,
        CTT_ID_PRODUCT_PS = 0x34,
        CTT_ID_PRODUCT_US = 0x35,
        CTT_ID_COLLECT_MINERAL = 0x36,
        CTT_ID_COLLECT_TREE = 0x37,
        CTT_ID_COLLECT_HERB = 0x38,
        CTT_ID_CLBETA_TITLE = 0x39,
        CTT_ID_GAME_DICE_WIN = 0x3A,
        CTT_ID_GAME_DICE_SET = 0x3B,
        CTT_ID_JOB_JOKER = 0x3C,
        CTT_ID_CC_HUMAR02 = 0x3D,
        CTT_ID_CC_C_JEWELGOLEM01 = 0x3E,
        CTT_ID_CC_HELGA01 = 0x3F,
        CTT_ID_CC_KAREN02 = 0x40,
        CTT_ID_CC_KALBANOBEB02 = 0x41,
        CTT_ID_CC_MARA02 = 0x42,
        CTT_ID_CC_SLIME02 = 0x43,
        CTT_ID_CC_LEVIATHAN01 = 0x44,
        CTT_ID_CC_DARKFOG = 0x45,
        CTT_ID_CC_ELFKNIGHTSOUL = 0x46,
        CTT_ID_CC_FIREPAMELIA = 0x47,
        CTT_ID_CC_GUARDIANMASTER01 = 0x48,
        CTT_ID_CC_HARPY01 = 0x49,
        CTT_ID_CC_WOLF = 0x4A,
        CTT_ID_CC_JEWELKEEPER = 0x4B,
        CTT_ID_CC_KINGCRAB02 = 0x4C,
        CTT_ID_CC_MAGRITE = 0x4D,
        CTT_ID_CC_MARLONE03 = 0x4E,
        CTT_ID_CC_ORC = 0x4F,
        CTT_ID_CC_PHINOFLIE = 0x50,
        CTT_ID_CC_SILBERK = 0x51,
        CTT_ID_CC_STONIE = 0x52,
        CTT_ID_CC_TOMBRAIDER03 = 0x53,
        CTT_ID_CC_BAT = 0x54,
        CTT_ID_CC_BOOGY02 = 0x55,
        CTT_ID_CC_SHELLA = 0x56,
        CTT_ID_CC_VIVI = 0x57,
        CTT_ID_CC_GREENKY = 0x58,
        CTT_ID_CC_ICELICH = 0x59,
        CTT_ID_CC_LAB_1902 = 0x5A,
        CTT_ID_CC_LAB_BATTLE0102 = 0x5B,
        CTT_ID_CC_LAB_WATCHMAN0102 = 0x5C,
        CTT_ID_CC_MARASAILOR01 = 0x5D,
        CTT_ID_CC_RATMAN02 = 0x5E,
        CTT_ID_CC_SHYLPH = 0x5F,
        CTT_ID_CC_SKELKNIGHT02 = 0x60,
        CTT_ID_CC_VAMPIREBAT = 0x61,
        CTT_ID_CC_CLOVERTRUMPY02 = 0x62,
        CTT_ID_CC_CLOVERTRUMPY03 = 0x63,
        CTT_ID_CC_EARTHCALERBEN = 0x64,
        CTT_ID_CC_FLYINGSTAFF = 0x65,
        CTT_ID_CC_HARKAN01 = 0x66,
        CTT_ID_CC_HONEYING03 = 0x67,
        CTT_ID_CC_KEBING01 = 0x68,
        CTT_ID_CC_LEIPOON = 0x69,
        CTT_ID_CC_LIVINGSTATUE02 = 0x6A,
        CTT_ID_CC_LIZARDMAN03 = 0x6B,
        CTT_ID_CC_MINEMOLE01 = 0x6C,
        CTT_ID_CC_NOX = 0x6D,
        CTT_ID_CC_SPADETRUMPY02 = 0x6E,
        CTT_ID_CC_SPARKDOG02 = 0x6F,
        CTT_ID_CC_ZOMBIE = 0x70,
        CTT_ID_JOB_SENTINEL = 0x71,
        CTT_ID_PZL_SLIME = 0x72,
        CTT_ID_PZL_HONEYING = 0x73,
        CTT_ID_PZL_SLIME_HONEYING = 0x74,
        CTT_ID_OLYMPIC_MEDAL_GOLD = 0x75,
        CTT_ID_OLYMPIC_MEDAL_SILVER = 0x76,
        CTT_ID_OLYMPIC_MEDAL_BRONZE = 0x77,
        CTT_ID_SOCCER_TOP_SCORER = 0x78,
        CTT_ID_SOCCER_PLAYER = 0x79,
        CTT_ID_H_WIN_MASTER = 0x7A,
        CTT_ID_H_WIN_GUIDE = 0x7B,
        CTT_ID_H_WIN_PASSER = 0x7C,
        CTT_ID_H_WIN_HELGA = 0x7D,
        CTT_ID_H_WIN_HUMAR = 0x7E,
        CTT_ID_H_WIN_JACKO = 0x7F,
        CTT_ID_UNION_QUEST = 0x80,
        CTT_ID_UNION_QUEST1 = 0x81,
        CTT_ID_UNION_QUEST2 = 0x82,
        CTT_ID_UNION_QUEST3 = 0x83,
        CTT_MAX_CHARACTER_TITLE_TYPE = 0x84
    }
}
