using System;
using Vision.Client.Enums;
using Vision.Client.Services;
using Vision.Core.Logging.Loggers;
using Vision.Core.Networking;
using Vision.Core.Networking.Packet;
using Vision.Game.Structs.Misc;

namespace Vision.Client.Networking.Handlers
{
    public static class MiscHandlers
    {
        private static readonly ClientLog ClientLogger = new ClientLog(typeof(MiscHandlers));
        private static readonly SocketLog SocketLogger = new SocketLog(typeof(MiscHandlers));

        private static readonly NetPacket HeartbeatAck = new NetPacket(NetCommand.NC_MISC_HEARTBEAT_ACK);

        [NetPacketHandler(NetCommand.NC_MISC_HEARTBEAT_REQ, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_MISC_HEARTBEAT_REQ(NetPacket packet, NetClientConnection connection)
        {
            HeartbeatAck.Send(connection);
            connection.LastPing = DateTime.Now;
        }

        [NetPacketHandler(NetCommand.NC_MISC_SEED_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_MISC_SEED_ACK(NetPacket packet, NetClientConnection connection)
        {
            var seed = packet.Reader.ReadUInt16();
            connection.Crypto.SetSeed(seed);
            SocketLogger.Debug($"Got seed packet from {connection.TransmitDestinationType.ToMessage()}. Seed: {seed}");

            switch (connection.TransmitDestinationType)
            {
                case NetConnectionDestination.NCD_LOGIN:
                    connection.UpdateLoginService(LoginServiceTrigger.LST_CONNECT_OK);
                    break;
                case NetConnectionDestination.NCD_WORLDMANAGER:
                    connection.UpdateWorldService(WorldServiceTrigger.WST_CONNECT_OK);
                    break;
                case NetConnectionDestination.NCD_ZONE:
                    connection.UpdateZoneService(ZoneServiceTrigger.ZST_CONNECT_OK);
                    break;
            }
        }

        [NetPacketHandler(NetCommand.NC_MISC_GAMETIME_ACK, NetConnectionDestination.NCD_CLIENT)]
        public static void NC_MISC_GAMETIME_ACK(NetPacket packet, NetClientConnection connection)
        {
            var ack = new NcGameTimeAck();
            ack.Read(packet);
            ClientLogger.Debug($"Got GameTime: {ack.GameTime}");

            switch (connection.TransmitDestinationType)
            {
                case NetConnectionDestination.NCD_WORLDMANAGER:
                    connection.GameClient.ClientSessionData.GameTime.Set(ack.GameTime);
                    break;
            }
        }
    }
}
