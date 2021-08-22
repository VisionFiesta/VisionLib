using Vision.Client;
using Vision.Client.Configuration;
using Vision.Core.Logging.Loggers;
using Console = Colorful.Console;

namespace VisionConsoleTesting
{
    public static class ClientTestProgram
    {
        private const int ConsoleWidth = 200;
        private const int ConsoleHeight = 50;

        private const string ConfigDir = "./Config";

        public static void Main(string[] arg)
        {
            // Console.SetWindowSize(ConsoleWidth, ConsoleHeight);
            // Console.SetBufferSize(ConsoleWidth, short.MaxValue - 1);

            ClientLog.SetLogLevel(ClientLogLevel.CLL_ERROR);
            EngineLog.SetLogLevel(EngineLogLevel.ELL_ERROR);
            SocketLog.SetLogLevel(SocketLogLevel.SLL_ERROR);

#if PKT2020
            Console.WriteLine("USING 2020 PACKETS");
#else
            Console.WriteLine("USING 2021 PACKETS");
#endif
            var userConf = new UserConfiguration(ConfigDir);
            var clientConf = new ClientConfiguration(ConfigDir, true);

            var client = new FiestaClient(userConf.Data[0], clientConf.Data);
            while (!client.Ready) { }
            client.Login();

            for (;;)
            {
                client.BusyLoop();
            }
        }
    }
}