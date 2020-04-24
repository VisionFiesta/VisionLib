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
            // TODO: move to ClientWorldService
            new FiestaNetPacket(FiestaNetCommand.NC_MAP_LOGINCOMPLETE_CMD).Send(connection);
            Log.Write(LogType.GameLog, LogLevel.Info, "Map Login OK");
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_MAP_LOGINFAIL_ACK, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_MAP_LOGINFAIL_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            Log.Write(LogType.GameLog, LogLevel.Error, "MapLoginFail");
            // TODO: move to ClientWorldService
            connection.Disconnect();
        }

        [FiestaNetPacketHandler(FiestaNetCommand.NC_MAP_LOGOUT_CMD, FiestaNetConnDest.FNCDEST_CLIENT)]
        public static void NC_MAP_LOGOUT_CMD(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            // Log.Write(LogType.GameLog, LogLevel.Warning, "MapLogout CMD");
        }
    }
}
