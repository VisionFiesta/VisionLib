using Vision.Client;
using Vision.Client.Configuration;
using Vision.Client.Enums;
using Vision.Core.Common.Logging;
using Vision.Core.Common.Logging.Loggers;

using Console = Colorful.Console;

namespace VisionConsoleTesting
{
    public class ClientTestProgram
    {
        private const int ConsoleWidth = 260;
        private const int ConsoleHeight = 50;

        public static void Main(string[] arg)
        {
            Console.SetBufferSize(ConsoleWidth, ConsoleHeight * 6);
            Console.SetWindowSize(ConsoleWidth, ConsoleHeight);

            Log.SetConsoleLogLevel(LogLevel.Error);
            // SocketLog.SetPreciseLogLevels(SocketLogLevel.SLL_UNHANDLED, SocketLogLevel.SLL_ERROR, SocketLogLevel.SLL_WARNING, SocketLogLevel.SLL_INFO);
            SocketLog.SetLogLevel(SocketLogLevel.SLL_UNHANDLED);
            // ClientLog.SetLogLevel(ClientLogLevel.CLL_ERROR);

            if (ClientConfiguration.Load(out _) && UserConfiguration.Load(out _) && GameConfiguration.Load(out _))
            {
                var client = new FiestaClient(UserConfiguration.Instance.Data, ClientConfiguration.Instance.Data, GameConfiguration.Instance);
                client.LoginService.SetStatus(ClientLoginServiceStatus.CLSS_TRYCONNECT);
            }

            Console.ReadKey();
        }
    }
}