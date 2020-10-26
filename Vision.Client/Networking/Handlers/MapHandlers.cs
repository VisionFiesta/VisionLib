using Vision.Client.Services;
using Vision.Core.Logging.Loggers;
using Vision.Core.Networking;
using Vision.Core.Networking.Packet;
using Vision.Game.Structs.Map;

namespace Vision.Client.Networking.Handlers
{
    public static class MapHandlers
    {
        private static readonly ClientLog Logger = new ClientLog(typeof(MapHandlers));

        [NetPacketHandler(NetCommand.NC_MAP_LOGIN_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_MAP_LOGIN_ACK(NetPacket packet, NetClientConnection connection)
        {
            var ack = new NcMapLoginAck();
            ack.Read(packet);
            Logger.Debug($"MAP_LOGIN_ACK: {ack}");
            connection.UpdateWorldService(WorldServiceTrigger.WST_LOGIN_ZONE_OK);
        }

        [NetPacketHandler(NetCommand.NC_MAP_LOGINFAIL_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_MAP_LOGINFAIL_ACK(NetPacket packet, NetClientConnection connection)
        {
            var ack = new NcMapLoginFailAck();
            ack.Read(packet);
            Logger.Debug($"MAP_LOGINFAIL_ACK: {ack}");
            connection.UpdateWorldService(WorldServiceTrigger.WST_LOGIN_ZONE_FAIL);
        }

        [NetPacketHandler(NetCommand.NC_MAP_LOGOUT_CMD, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_MAP_LOGOUT_CMD(NetPacket packet, NetClientConnection connection)
        {
            var handle = packet.Reader.ReadUInt16();
            // Logger.Debug($"MAP_LOGOUT_CMD: Handle {handle}");
        }
    }
}
