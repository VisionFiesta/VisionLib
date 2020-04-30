using Vision.Core.Networking;
using Vision.Core.Networking.Packet;

namespace Vision.Client.Networking.Handlers
{
    public static class KQHandlers
    {
        [FiestaNetPacketHandler(FiestaNetCommand.NC_KQ_JOINING_ALARM_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_KQ_JOINING_ALARM_CMD(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {

        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_KQ_JOINING_ALARM_END_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_KQ_JOINING_ALARM_END_CMD(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {

        }
    }
}
