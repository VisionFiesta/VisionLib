using System.Collections.Generic;
using Vision.Client.Data;

namespace Vision.Client.Enums
{
    public enum ClientYear
    {
        CV_2007,
        CV_2008,
        CV_2012,
        CV_2015,
        CV_2016,
        CV_2017,
        CV_2020
    }

    public static class ClientVersionData
    {
        public static Dictionary<ClientYear, ClientVersionInfo> VersionInfos = new Dictionary<ClientYear, ClientVersionInfo>();
    }
}
