using System;
using System.Linq;
using System.Reflection;
using VisionLib.Common.Collections;
using VisionLib.Common.Logging;

namespace VisionLib.Common.Networking.Packet
{
    public static class FiestaNetPacketHandlerLoader
    {
        public static readonly FastDictionary<FiestaNetCommand, FiestaNetPacketHandlerDelegate> Handlers = new FastDictionary<FiestaNetCommand, FiestaNetPacketHandlerDelegate>();

        public static void LoadHandlers()
        {
            var allMethods = VisionAssembly.GetMethodsWithAttribute<FiestaNetPacketHandlerAttritube>().ToList();
            foreach (Pair<FiestaNetPacketHandlerAttritube, MethodInfo> pair in allMethods)
            {
                FiestaNetPacketHandlerAttritube first = pair.First;
                MethodInfo second = pair.Second;
                if (Handlers.ContainsKey(first.Command))
                {
                    Log.Write(LogType.EngineLog, LogLevel.Warning, $"Duplicate message handler found: [{first.Command}]");
                    Handlers.Remove(first.Command);
                }
                Handlers.Add(first.Command, (FiestaNetPacketHandlerDelegate)Delegate.CreateDelegate(typeof(FiestaNetPacketHandlerDelegate), second));
                Log.Write(LogType.EngineLog, LogLevel.Debug, $"Added message handler: (Command: {first.Command}, Method: {second.Name})");
            }
        }

        public static bool TryGetHandler(FiestaNetCommand command, out FiestaNetPacketHandlerDelegate handler)
        {
            return Handlers.TryGetValue(command, out handler);
        }
    }
}
