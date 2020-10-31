using System;
using System.Collections.Generic;
using System.Linq;

namespace Vision.Core
{
    using GR_KVP_Short = KeyValuePair<GameRegion, string>;

    public enum GameRegion
    {
        GR_NA,
        GR_DE,
        GR_TW,
        GR_KR,
        GR_JP,
        GR_CN,
    }

    public static class GameRegionExtensions
    {
        public static string ToPacketShortName(this GameRegion region)
        {
            // todo: find the rest of these
            return region switch
            {
                GameRegion.GR_NA => "US",
                GameRegion.GR_DE => "GER",
                GameRegion.GR_TW => "TW",
                GameRegion.GR_KR => "KR",
                GameRegion.GR_JP => "JP",
                GameRegion.GR_CN => "CN",
                _ => "UNK"
            };
        }

        public static string ToShortName(this GameRegion region)
        {
            return region switch
            {
                GameRegion.GR_NA => "NA",
                GameRegion.GR_DE => "DE",
                GameRegion.GR_TW => "TW",
                GameRegion.GR_KR => "KR",
                GameRegion.GR_JP => "JP",
                GameRegion.GR_CN => "CN",
                _ => "UNK"
            };
        }

        public static string ToName(this GameRegion region)
        {
            return region switch
            {
                GameRegion.GR_NA => "North America",
                GameRegion.GR_DE => "Germany",
                GameRegion.GR_TW => "Taiwan",
                GameRegion.GR_KR => "Korea",
                GameRegion.GR_JP => "Japan",
                GameRegion.GR_CN => "China",
                _ => "Unknown Region"
            };
        }

        public static readonly List<GR_KVP_Short> KVPListByShortName =
            Enum.GetValues(typeof(GameRegion)).Cast<GameRegion>().Select(o => new GR_KVP_Short(o, o.ToShortName())).ToList();

    }
}
