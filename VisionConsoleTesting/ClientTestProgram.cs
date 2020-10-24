
using Vision.Client;
using Vision.Client.Configuration;
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

            ClientLog.SetLogLevel(ClientLogLevel.CLL_DEBUG);
            EngineLog.SetLogLevel(EngineLogLevel.ELL_ERROR);
            SocketLog.SetLogLevel(SocketLogLevel.SLL_ERROR);

            if (ClientConfiguration.Load(out _) && UserConfiguration.Load(out _))
            {
                var cud = UserConfiguration.Instance.Data;
                var client = new FiestaClient(cud[1]);
                client.LoginService.UpdateState(Vision.Client.Services.LoginServiceTrigger.LST_TRY_CONNECT);
            }

            Console.ReadLine();
        }
    }
}