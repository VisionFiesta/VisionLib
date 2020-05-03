using Vision.Client;
using Vision.Client.Configuration;
using Vision.Core;
using Vision.Core.Logging.Loggers;
using Console = Colorful.Console;

namespace VisionConsoleTesting
{
    public class ClientTestProgram
    {
        private const int ConsoleWidth = 200;
        private const int ConsoleHeight = 50;

        public static void Main(string[] arg)
        {
            Console.SetWindowSize(ConsoleWidth, ConsoleHeight);
            Console.SetBufferSize(ConsoleWidth, ConsoleHeight * 6);

            EngineLog.SetLogLevel(EngineLogLevel.ELL_ERROR);
            SocketLog.SetLogLevel(SocketLogLevel.SLL_ERROR);

            if (ClientConfiguration.Load(out _) && UserConfiguration.Load(out _) && GameConfiguration.Load(out _))
            {
                var client = new FiestaClient(UserConfiguration.Instance.Data, ClientConfiguration.Instance.Data, GameConfiguration.Instance);
                // client.LoginService.SetStatus(ClientLoginServiceStatus.CLSS_TRYCONNECT);
            }

            Console.ReadLine();
        }
    }
}