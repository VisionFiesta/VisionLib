
using System;
using Vision.Client;
using Vision.Client.Configuration;
using Vision.Core.Logging.Loggers;
using Vision.Game.Structs.Act;
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
            Console.SetBufferSize(ConsoleWidth, short.MaxValue - 1);

            ClientLog.SetLogLevel(ClientLogLevel.CLL_ERROR);
            EngineLog.SetLogLevel(EngineLogLevel.ELL_ERROR);
            SocketLog.SetLogLevel(SocketLogLevel.SLL_ERROR);

#if PKT2020
            Console.WriteLine("USING 2020 PACKETS");
#else
            Console.WriteLine("USING 2021 PACKETS");
#endif
            // if (ClientConfiguration.Load(out _) && UserConfiguration.Load(out _))
            // {
            // var cud = UserConfiguration.Instance.Data;
            // var client = new FiestaClient(cud[1]);
            // client.LoginService.UpdateState(Vision.Client.Services.LoginServiceTrigger.LST_TRY_CONNECT);
            // }

            Console.ReadLine();
        }
    }
}