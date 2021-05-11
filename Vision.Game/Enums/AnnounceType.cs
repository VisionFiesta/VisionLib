using System;
using System.Collections.Generic;
using System.Linq;

namespace Vision.Game.Enums
{
    public enum AnnounceType : byte
    {
        AT_ENHANCE = 1,
        AT_ACQUIRE = 2,
        AT_PRODUCE = 3,
        AT_LV20 = 4,
        AT_PROMOTE = 5,
        AT_TITLEACQUIRE = 6,
        AT_PETEVOLVE = 7,
        AT_GUILDWARBEGIN = 8,
        AT_GUILDWAREND = 9,
        AT_GUILDRANKUP = 10,
        AT_ROAR = 11,
        AT_PROPOSALACCEPT = 12,
        AT_MARRIAGE = 13
    }

    public static class AnnounceTypeExtensions
    {
        public static string ToFriendlyName(this AnnounceType type)
        {
            

            return type switch
            {
                AnnounceType.AT_ENHANCE => "Enhancement",
                AnnounceType.AT_ACQUIRE => "Rare Drop",
                AnnounceType.AT_PRODUCE => "Production",
                AnnounceType.AT_LV20 => "Level 20",
                AnnounceType.AT_PROMOTE => "Promotion",
                AnnounceType.AT_TITLEACQUIRE => "Rare Title",
                AnnounceType.AT_PETEVOLVE => "Pet Evolve",
                AnnounceType.AT_GUILDWARBEGIN => "Guild War Begin",
                AnnounceType.AT_GUILDWAREND => "Guild War End",
                AnnounceType.AT_GUILDRANKUP => "Guild Rank Up",
                AnnounceType.AT_ROAR => "Roar",
                AnnounceType.AT_PROPOSALACCEPT => "Marriage Acceptance",
                AnnounceType.AT_MARRIAGE => "Marriage Proposal",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}
