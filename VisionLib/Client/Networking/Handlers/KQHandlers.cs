using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Packet;

namespace VisionLib.Client.Networking.Handlers
{
    public static class KQHandlers
    {
        [FiestaNetPacketHandler(FiestaNetCommand.NC_KQ_JOINING_ALARM_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_KQ_JOINING_ALARM_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {

        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_KQ_JOINING_ALARM_END_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_KQ_JOINING_ALARM_END_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {

        }
    }
}
