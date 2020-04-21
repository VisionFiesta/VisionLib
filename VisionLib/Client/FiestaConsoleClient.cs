using VisionLib.Client.Configuration;
using VisionLib.Common.Logging;
using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Packet;

namespace VisionLib.Client
{
    public class FiestaConsoleClient
    {
        public static ClientConfiguration Config = new ClientConfiguration();
        public static FiestaNetConnection LoginClient = new FiestaNetConnection(FiestaNetConnDest.FNCDEST_LOGIN, FiestaNetConnDir.FNCDIR_TO_SERVER);
        public static FiestaNetConnection WorldClient = new FiestaNetConnection(FiestaNetConnDest.FNCDEST_WORLDMANAGER, FiestaNetConnDir.FNCDIR_TO_SERVER);
        public static FiestaNetConnection ZoneClient = new FiestaNetConnection(FiestaNetConnDest.FNCDEST_ZONE, FiestaNetConnDir.FNCDIR_TO_SERVER);

        public static class LoginData
        {
            public static byte[] WmTransferKey;
            public static ushort WmHandle;
            public static string WorldIP;
            public static ushort WorldPort;
        }

        public static class WorldData
        {

        }

        public static class ZoneData
        {

        }

        public static void Begin()
        {
            FiestaNetPacketHandlerLoader.LoadHandlers();
            if (!Config.Load(out var message))
            {
                Log.Write(LogType.EngineLog, LogLevel.Error, "Config load failed! Message: " + message);
            }
            else
            {
                Config = ClientConfiguration.Instance;
            }
        }
    }
}
