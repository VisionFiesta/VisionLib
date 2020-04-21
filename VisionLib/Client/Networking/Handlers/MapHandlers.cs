using VisionLib.Common.Logging;
using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Packet;

namespace VisionLib.Client.Networking.Handlers
{
    public static class MapHandlers
    {
        [FiestaNetPacketHandlerAttritube(FiestaNetCommand.NC_MAP_LOGIN_ACK)]
        public static void NC_MAP_LOGIN_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            new FiestaNetPacket(FiestaNetCommand.NC_MAP_LOGINCOMPLETE_CMD).Send(connection);
            Log.Write(LogType.GameLog, LogLevel.Info, "YEET");
        }

        [FiestaNetPacketHandlerAttritube(FiestaNetCommand.NC_MAP_LOGINFAIL_ACK)]
        public static void NC_MAP_LOGINFAIL_ACK(FiestaNetPacket packet, FiestaNetConnection connection)
        {
            // new FiestaNetPacket(FiestaNetCommand.NC_MAP_LOGINCOMPLETE_CMD).Send(connection);
            Log.Write(LogType.GameLog, LogLevel.Error, "MapLoginFail");
        }
    }
}
