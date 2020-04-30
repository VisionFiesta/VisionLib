using Vision.Core.Networking;
using Vision.Core.Networking.Packet;

namespace Vision.Client.Networking.Handlers
{
    public static class BatHandlers
    {
        [NetPacketHandler(NetCommand.NC_BAT_ABSTATESET_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BAT_ABSTATESET_CMD(NetPacket packet, NetClientConnection connection)
        {
            // TODO
        }

        [NetPacketHandler(NetCommand.NC_BAT_DOTDAMAGE_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BAT_DOTDAMAGE_CMD(NetPacket packet, NetClientConnection connection)
        {
            // TODO
        }

        [NetPacketHandler(NetCommand.NC_BAT_CEASE_FIRE_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BAT_CEASE_FIRE_CMD(NetPacket packet, NetClientConnection connection)
        {
            // TODO
        }

        [NetPacketHandler(NetCommand.NC_BAT_ABSTATEINFORM_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_BAT_ABSTATEINFORM_CMD(NetPacket packet, NetClientConnection connection)
        {
            // TODO
        }
    }
}
