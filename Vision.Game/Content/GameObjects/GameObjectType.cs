using System;

namespace Vision.Game.Content.GameObjects
{
    public enum GameObjectType
    {
        GOT_FLAG,
        GOT_DROPITEM,
        GOT_CHARACTER,
        GOT_MINIHOUSE,
        GOT_NPC,
        GOT_MOB,
        GOT_MAGICFIELD,
        GOT_DOOR,
        GOT_BANDIT,
        GOT_EFFECT,
        GOT_SERVANT,
        GOT_MOVER,
        GOT_PET,
        GOT_MAX,
    }

    public static class GameObjectTypeExtensions
    {
        public static string ToFriendlyName(this GameObjectType type)
        {
            switch (type)
            {
                case GameObjectType.GOT_MOB: return "Monster";
                case GameObjectType.GOT_NPC: return "NPC";
                case GameObjectType.GOT_DOOR: return "Gate";
                default: return type.ToString();
            }
        }
    }
}
