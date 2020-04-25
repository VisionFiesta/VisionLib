using VisionLib.Common.Extensions;
using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Packet;
using VisionLib.Core.Struct.Char;

namespace VisionLib.Client.Networking.Handlers
{
    public static class CharHandlers
    {
        [FiestaNetPacketHandler(FiestaNetCommand.NC_CHAR_LOGIN_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_CHAR_LOGIN_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            var ack = new NcCharLoginAck();
            ack.Read(packet.Reader);
            // TODO: Move to ClientZoneService
            connection.GetClient()?.ZoneClient.Connect(ack.ZoneEndPoint);
        }
    }
}
