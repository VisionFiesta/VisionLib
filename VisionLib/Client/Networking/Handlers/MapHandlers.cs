using VisionLib.Client.Enums;
using VisionLib.Common.Extensions;
using VisionLib.Common.Logging;
using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Packet;

namespace VisionLib.Client.Networking.Handlers
{
    public static class MapHandlers
    {
        [FiestaNetPacketHandler(FiestaNetCommand.NC_MAP_LOGIN_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_MAP_LOGIN_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            // TODO: move to ClientZoneService
            new FiestaNetPacket(FiestaNetCommand.NC_MAP_LOGINCOMPLETE_CMD).Send(connection);
            ClientLog.Info("Map Login OK");
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_MAP_LOGINFAIL_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_MAP_LOGINFAIL_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            ClientLog.Warning("Map Login Failed");
            connection.GetClient()?.WorldService.SetStatus(ClientWorldServiceStatus.CWSS_NOTCONNECTED);
            connection.Disconnect();
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_MAP_LOGOUT_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_MAP_LOGOUT_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            // when other MapObject logs out
        }
    }
}
