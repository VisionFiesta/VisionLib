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
            Log.Write(LogType.GameLog, LogLevel.Debug, $"Got seed packet from {connection.TransmitDestinationType.ToMessage()}. Seed: {seed}");

            switch (connection.TransmitDestinationType)
            {
                case FiestaNetConnDest.FNCDEST_LOGIN:
                    connection.GetClient()?.LoginService.SetStatus(ClientLoginServiceStatus.CLSS_CONNECTED);
                    break;
                case FiestaNetConnDest.FNCDEST_WORLDMANAGER:
                    connection.GetClient()?.WorldService.SetStatus(ClientWorldServiceStatus.CWSS_CONNECTED);
                    break;
                case FiestaNetConnDest.FNCDEST_ZONE:
                    // TODO: Move to ClientZoneService
                    // new STRUCT_MAP_LOGIN_REQ(FiestaConsoleClient.LoginData.WmHandle, "SVT_0001", FiestaConsoleClient.Config.SHNHash).ToPacket().Send(connection);
                    break;
                case FiestaNetConnDest.FNCDEST_CLIENT:
                    break;
            }
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_MISC_GAMETIME_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_MISC_GAMETIME_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var ack = new NcGameTimeAck();
            ack.Read(packet.Reader);
            Log.Write(LogType.GameLog, LogLevel.Info, $"Got GameTime: {ack.GameTime}");

            switch (connection.TransmitDestinationType)
            {
                case FiestaNetConnDest.FNCDEST_WORLDMANAGER:
                    // TODO: Move to ClientWorldService
                    // new STRUCT_NC_CHAR_LOGIN_REQ(0).ToPacket().Send(connection);
                    break;
            }
        }
    }
}
