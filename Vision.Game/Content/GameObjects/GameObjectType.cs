using System.Diagnostics.CodeAnalysis;

namespace Vision.Game.Content.GameObjects
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
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
            return type switch
            {
                GameObjectType.GOT_MOB => "Monster",
                GameObjectType.GOT_NPC => "NPC",
                GameObjectType.GOT_DOOR => "Gate",
                GameObjectType.GOT_CHARACTER => "Character",
                GameObjectType.GOT_MOVER => "Mover",
                _ => type.ToString()
            };
        }
    }
}
