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
        [FiestaNetPacketHandler(FiestaNetCommand.NC_MISC_SEED_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_MISC_SEED_ACK(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            var seed = packet.Reader.ReadUInt16();
            connection.Crypto.SetSeed(seed);
            SocketLog.Debug($"Got seed packet from {connection.TransmitDestinationType.ToMessage()}. Seed: {seed}");

            switch (connection.TransmitDestinationType)
            {
                case FiestaNetConnDest.FNCDEST_LOGIN:
                    connection.GameClient.LoginService.SetStatus(ClientLoginServiceStatus.CLSS_CONNECTED);
                    break;
                case FiestaNetConnDest.FNCDEST_WORLDMANAGER:
                    connection.GameClient.WorldService.SetStatus(ClientWorldServiceStatus.CWSS_CONNECTED);
                    break;
                case FiestaNetConnDest.FNCDEST_ZONE:
                    connection.GameClient.ZoneService.SetStatus(ClientZoneServiceStatus.CZSS_CONNECTED);
                    break;
            }
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_MISC_GAMETIME_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_MISC_GAMETIME_ACK(FiestaNetPacket packet, FiestaNetClientConnection connection)
        {
            var ack = new NcGameTimeAck();
            ack.Read(packet);
            ClientLog.Debug($"Got GameTime: {ack.GameTime}");

            switch (connection.TransmitDestinationType)
            {
                case FiestaNetConnDest.FNCDEST_WORLDMANAGER:
                    connection.GameClient.GameData.GameTime.Set(ack.GameTime);
                    connection.GameClient.WorldService.SetStatus(ClientWorldServiceStatus.CWSS_GOTGAMETIME);
                    break;
            }
        }
    }
}
