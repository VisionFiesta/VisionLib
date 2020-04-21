using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Packet;
using VisionLib.Common.Networking.Structs.Char;

namespace VisionLib.Client.Networking.Handlers
{
    public static class CharHandlers
    {
        [FiestaNetPacketHandlerAttritube(FiestaNetCommand.NC_CHAR_LOGIN_ACK)]
        public static void NC_CHAR_LOGIN_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var result = new STRUCT_NC_CHAR_LOGIN_ACK(packet);
            FiestaConsoleClient.ZoneClient.Connect(result.ZoneIP, result.ZonePort);
        }
    }
}
