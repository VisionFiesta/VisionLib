using System;
using System.Linq;
using Vision.Core.Common;
using Vision.Core.Common.Collections;
using Vision.Core.Common.Logging;

namespace Vision.Core.Networking.Packet
{
    public static class FiestaNetPacketHandlerLoader<T> where T : FiestaNetConnection
    {
        public static readonly FastDictionary<FiestaNetCommand, Tuple<FiestaNetConnDest[], FiestaNetPacketHandlerDelegate<T>>> Handlers = new FastDictionary<FiestaNetCommand, Tuple<FiestaNetConnDest[], FiestaNetPacketHandlerDelegate<T>>>();
        // public static readonly FastDictionary<FiestaNetCommand, TypeInfo> Structs = new FastDictionary<FiestaNetCommand, TypeInfo>();

        public static void LoadHandlers()
        {
            var allMethods = VisionAssembly.GetMethodsWithAttribute<FiestaNetPacketHandlerAttribute>().ToList();
            foreach (var pair in allMethods)
            {
                var first = pair.First;
                var second = pair.Second;
                if (Handlers.ContainsKey(first.Command))
                {
                    Log.Write(LogType.EngineLog, LogLevel.Warning, $"Duplicate message handler found: [{first.Command}]");
                    Handlers.Remove(first.Command);
                }

                var destinations = first.Destinations;
                var theDelegate =
                    Delegate.CreateDelegate(typeof(FiestaNetPacketHandlerDelegate<T>),
                        second) as FiestaNetPacketHandlerDelegate<T>;
                var both = new Tuple<FiestaNetConnDest[], FiestaNetPacketHandlerDelegate<T>>(destinations, theDelegate);

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

            // var allStructs = VisionAssembly.GetTypesOfBase<NetPacketStruct>().ToList();
            //
            // foreach (var @struct in allStructs)
            // {
            //     var ctor = @struct.GetConstructor(Type.EmptyTypes);
            //     if (ctor == null) continue;
            //     var created = ctor.Invoke(new object[] { });
            //     var command = (FiestaNetCommand)@struct.GetDeclaredMethod("GetCommand").Invoke(created, null);
            //     Structs.Add(command, @struct);
            //
            //     Log.Write(LogType.EngineLog, LogLevel.Debug, $"Added struct: (Command: {command}, Struct: {@struct.Name}");
            // }
        }

        public static bool TryGetHandler(FiestaNetCommand command, out FiestaNetPacketHandlerDelegate<T> handler, out FiestaNetConnDest[] destinations)
        {
            var result = Handlers.TryGetValue(command, out var both);
            if (result)
            {
                handler = both.Item2;
                destinations = both.Item1;
                return true;
            }

            handler = null;
            destinations = null;
            return false;
        }
    }
}
