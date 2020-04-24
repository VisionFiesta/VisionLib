using System;
using System.Linq;
using System.Reflection;
using VisionLib.Common.Collections;
using VisionLib.Common.Logging;

namespace VisionLib.Common.Networking.Packet
{
    public static class FiestaNetPacketHandlerLoader
    {
        public static readonly FastDictionary<FiestaNetCommand, Tuple<FiestaNetConnDest[], FiestaNetPacketHandlerDelegate>> Handlers  = new FastDictionary<FiestaNetCommand, Tuple<FiestaNetConnDest[], FiestaNetPacketHandlerDelegate>>();

        public static void LoadHandlers()
        {
            var allMethods = VisionAssembly.GetMethodsWithAttribute<FiestaNetPacketHandlerAttribute>().ToList();
            foreach (Pair<FiestaNetPacketHandlerAttribute, MethodInfo> pair in allMethods)
            {
                FiestaNetPacketHandlerAttribute first = pair.First;
                MethodInfo second = pair.Second;
                if (Handlers.ContainsKey(first.Command))
                {
                    Log.Write(LogType.EngineLog, LogLevel.Warning, $"Duplicate message handler found: [{first.Command}]");
                    Handlers.Remove(first.Command);
                }

                var destinations = first.Destinations;
                var theDelegate =
                    Delegate.CreateDelegate(typeof(FiestaNetPacketHandlerDelegate),
                        second) as FiestaNetPacketHandlerDelegate;
                var both = new Tuple<FiestaNetConnDest[], FiestaNetPacketHandlerDelegate>(destinations, theDelegate);

                Handlers.Add(first.Command, both);

                var destinationsStr = "";
                for (var index = 0; index < destinations.Length; index++)
                {
                    var dest = destinations[index];
                    destinationsStr += dest.ToMessage();
                    if (index != destinations.Length - 1) destinationsStr += ", ";
                }

                Log.Write(LogType.EngineLog, LogLevel.Debug, $"Added message handler: (Command: {first.Command}, Destinations: {destinationsStr})");
            }
        }

        public static bool TryGetHandler(FiestaNetCommand command, out FiestaNetPacketHandlerDelegate handler, out FiestaNetConnDest[] destinations)
        {
            var result = Handlers.TryGetValue(command, out var both);
            if (result)
            {
                handler = both.Item2;
                destinations = both.Item1;
                return true;
            }
            else
            {
                handler = null;
                destinations = null;
                return false;
            }
        }
    }
}
