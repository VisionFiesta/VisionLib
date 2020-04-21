using VisionLib.Client.Enums;
using VisionLib.Client.Services;
using VisionLib.Common.Logging;
using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Crypto;
using VisionLib.Common.Networking.Packet;
using VisionLib.Common.Networking.Structs.Char;
using VisionLib.Common.Networking.Structs.Map;
using VisionLib.Common.Networking.Structs.Misc;
using VisionLib.Common.Networking.Structs.User;

namespace VisionLib.Client.Networking.Handlers
{
    public static class MiscHandlers
    {
        [FiestaNetPacketHandlerAttritube(FiestaNetCommand.NC_MISC_SEED_ACK)]
        public static void NC_MISC_SEED_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var seed = packet.ReadUInt16();
            Log.Write(LogType.GameLog, LogLevel.Debug, $"Got seed packet from {connection.DestinationType.ToMessage()}. Seed: {seed}");
            ((FiestaNetCrypto_NA2020)connection.Crypto).SetSeed(seed);

            switch (connection.DestinationType)
            {
                case FiestaNetConnDest.FNCDEST_LOGIN:
                    LoginService.SetStatus(ClientLoginStatus.CLS_CONNECTED);
                    break;
                case FiestaNetConnDest.FNCDEST_WORLDMANAGER:
                    new STRUCT_NC_USER_LOGINWORLD_REQ(FiestaConsoleClient.Config.FiestaUsername, FiestaConsoleClient.LoginData.WmTransferKey).ToPacket().Send(connection);
                    break;
                case FiestaNetConnDest.FNCDEST_ZONE:
                    new STRUCT_MAP_LOGIN_REQ(FiestaConsoleClient.LoginData.WmHandle, "SVT_0001", FiestaConsoleClient.Config.SHNHash).ToPacket().Send(connection);
                    break;
                case FiestaNetConnDest.FNCDEST_CLIENT:
                    break;
            }
        }

        [FiestaNetPacketHandlerAttritube(FiestaNetCommand.NC_MISC_GAMETIME_ACK)]
        public static void NC_MISC_GAMETIME_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var ack = new STRUCT_GAMETIME_ACK(packet);

            switch (connection.DestinationType)
            {
                case FiestaNetConnDest.FNCDEST_WORLDMANAGER:
                    new STRUCT_NC_CHAR_LOGIN_REQ(0).ToPacket().Send(connection);
                    break;
            }
        }
    }
}
