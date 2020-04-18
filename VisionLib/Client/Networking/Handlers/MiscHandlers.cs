using VisionLib.Client;
using VisionLib.Common.Logging;
using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Structs.User;

namespace VisionLib.Common.Network.Packets.Misc
{
    public static class MiscHandlers
    {
        [FiestaNetPacketHandlerAttritube(FiestaNetCommand.NC_MISC_SEED_ACK)]
        public static void NC_MISC_SEED_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var seed = packet.ReadUInt16();
            Log.Write(LogType.GameLog, LogLevel.Info, $"Got seed packet from {connection.DestinationType.ToMessage()}. Seed: {seed}");
            ((FiestaNetCrypto_NA2020)connection.Crypto).SetSeed(seed);

            switch (connection.DestinationType)
            {
                case FiestaNetConnDest.FNCDEST_LOGIN:
                    new STRUCT_NC_USER_CLIENT_VERSION_CHECK_REQ(FiestaClient.Config.BinMD5, FiestaClient.Config.ClientVersionData).ToPacket().Send(connection);
                    break;
                case FiestaNetConnDest.FNCDEST_WORLDMANAGER:
                    new STRUCT_NC_USER_LOGINWORLD_REQ(FiestaClient.Config.FiestaUsername, FiestaClient.ConnectionHash).ToPacket().Send(connection);
                    break;
                case FiestaNetConnDest.FNCDEST_ZONE:
                    break;
                case FiestaNetConnDest.FNCDEST_CLIENT:
                    break;
            }
        }
    }
}
