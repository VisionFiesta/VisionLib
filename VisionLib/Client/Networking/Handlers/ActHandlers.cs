using System;
using System.Collections.Generic;
using System.Text;
using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Packet;

namespace VisionLib.Client.Networking.Handlers
{
    public static class ActHandlers
    {
        [FiestaNetPacketHandler(FiestaNetCommand.NC_ACT_SOMEONEMOVEWALK_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_ACT_SOMEONEMOVEWALK_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            // TODO
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_ACT_SOMEONEMOVERUN_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_ACT_SOMEONEMOVERUN_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            // TODO
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_ACT_SOMEONESTOP_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_ACT_SOMEONESTOP_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            // TODO
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_ACT_SOMEONEPRODUCE_MAKE_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_ACT_SOMEONEPRODUCE_MAKE_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            // TODO
        }
    }
}
