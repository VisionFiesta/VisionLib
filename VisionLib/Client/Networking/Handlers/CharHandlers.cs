using VisionLib.Common.Extensions;
using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Packet;
using VisionLib.Common.Networking.Structs.Char;

namespace VisionLib.Client.Networking.Handlers
{
    public static class CharHandlers
    {
        [FiestaNetPacketHandler(FiestaNetCommand.NC_CHAR_LOGIN_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_CHAR_LOGIN_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var result = new STRUCT_NC_CHAR_LOGIN_ACK(packet);
            // TODO: Move to ClientZoneService
            connection.GetClient()?.ZoneClient.Connect(result.ZoneEndPoint);
        }
    }
}
