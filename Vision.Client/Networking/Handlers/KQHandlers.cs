using Vision.Core.Networking;
using Vision.Core.Networking.Packet;

namespace Vision.Client.Networking.Handlers
{
    public static class KQHandlers
    {
        [NetPacketHandler(NetCommand.NC_KQ_JOINING_ALARM_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_KQ_JOINING_ALARM_CMD(NetPacket packet, NetClientConnection connection)
        {

        }

        [NetPacketHandler(NetCommand.NC_KQ_JOINING_ALARM_END_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_KQ_JOINING_ALARM_END_CMD(NetPacket packet, NetClientConnection connection)
        {

        }
    }
}
