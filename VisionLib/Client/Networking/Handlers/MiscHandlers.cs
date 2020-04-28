using VisionLib.Client.Enums;
using VisionLib.Common.Extensions;
using VisionLib.Common.Logging;
using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Packet;
using VisionLib.Core.Struct.Misc;

namespace VisionLib.Client.Networking.Handlers
{
    public static class MiscHandlers
    {
        [FiestaNetPacketHandler(FiestaNetCommand.NC_MISC_SEED_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_MISC_SEED_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var seed = packet.Reader.ReadUInt16();
            connection.Crypto.SetSeed(seed);
            SocketLog.Debug($"Got seed packet from {connection.TransmitDestinationType.ToMessage()}. Seed: {seed}");

            switch (connection.TransmitDestinationType)
            {
                case FiestaNetConnDest.FNCDEST_LOGIN:
                    connection.GetClient()?.LoginService.SetStatus(ClientLoginServiceStatus.CLSS_CONNECTED);
                    break;
                case FiestaNetConnDest.FNCDEST_WORLDMANAGER:
                    connection.GetClient()?.WorldService.SetStatus(ClientWorldServiceStatus.CWSS_CONNECTED);
                    break;
                case FiestaNetConnDest.FNCDEST_ZONE:
                    connection.GetClient()?.ZoneService.SetStatus(ClientZoneServiceStatus.CZSS_CONNECTED);
                    break;
            }
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_MISC_GAMETIME_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_MISC_GAMETIME_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var ack = new NcGameTimeAck();
            ack.Read(packet);
            ClientLog.Debug($"Got GameTime: {ack.GameTime}");

            switch (connection.TransmitDestinationType)
            {
                case FiestaNetConnDest.FNCDEST_WORLDMANAGER:
                    connection.GetClient()?.GameData.GameTime.Set(ack.GameTime);
                    connection.GetClient()?.WorldService.SetStatus(ClientWorldServiceStatus.CWSS_GOTGAMETIME);
                    break;
            }
        }
    }
}
