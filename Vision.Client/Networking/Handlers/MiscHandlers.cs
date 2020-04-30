using Vision.Client.Enums;
using Vision.Core.Common.Logging.Loggers;
using Vision.Core.Logging.Loggers;
using Vision.Core.Networking;
using Vision.Core.Networking.Packet;
using Vision.Game.Structs.Misc;

namespace Vision.Client.Networking.Handlers
{
    public static class MiscHandlers
    {
        private static readonly NetPacket HeartbeatAck = new NetPacket(NetCommand.NC_MISC_HEARTBEAT_ACK);

        [NetPacketHandler(NetCommand.NC_MISC_HEARTBEAT_REQ, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_MISC_HEARTBEAT_REQ(NetPacket packet, NetClientConnection connection)
        {
            HeartbeatAck.Send(connection);
        }

        [NetPacketHandler(NetCommand.NC_MISC_SEED_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_MISC_SEED_ACK(NetPacket packet, NetClientConnection connection)
        {
            var seed = packet.Reader.ReadUInt16();
            connection.Crypto.SetSeed(seed);
            SocketLog.Debug($"Got seed packet from {connection.TransmitDestinationType.ToMessage()}. Seed: {seed}");

            switch (connection.TransmitDestinationType)
            {
                case NetConnectionDestination.NCD_LOGIN:
                    connection.GameClient.LoginService.SetStatus(ClientLoginServiceStatus.CLSS_CONNECTED);
                    break;
                case NetConnectionDestination.NCD_WORLDMANAGER:
                    connection.GameClient.WorldService.SetStatus(ClientWorldServiceStatus.CWSS_CONNECTED);
                    break;
                case NetConnectionDestination.NCD_ZONE:
                    connection.GameClient.ZoneService.SetStatus(ClientZoneServiceStatus.CZSS_CONNECTED);
                    break;
            }
        }

        [NetPacketHandler(NetCommand.NC_MISC_GAMETIME_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_MISC_GAMETIME_ACK(NetPacket packet, NetClientConnection connection)
        {
            var ack = new NcGameTimeAck();
            ack.Read(packet);
            ClientLog.Debug($"Got GameTime: {ack.GameTime}");

            switch (connection.TransmitDestinationType)
            {
                case NetConnectionDestination.NCD_WORLDMANAGER:
                    connection.GameClient.GameData.GameTime.Set(ack.GameTime);
                    connection.GameClient.WorldService.SetStatus(ClientWorldServiceStatus.CWSS_GOTGAMETIME);
                    break;
            }
        }
    }
}
