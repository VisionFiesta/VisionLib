using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Vision.Core.Logging.Loggers;

namespace Vision.Core.Networking.Packet
{
    public static class NetPacketHandlerLoader<T> where T : NetConnectionBase<T>
    {
        private static readonly Dictionary<NetCommand, Tuple<NetConnectionDestination[], NetPacketHandlerDelegate<T>>> Handlers = new Dictionary<NetCommand, Tuple<NetConnectionDestination[], NetPacketHandlerDelegate<T>>>();
        // public static readonly FastDictionary<FiestaNetCommand, TypeInfo> Structs = new FastDictionary<FiestaNetCommand, TypeInfo>();

        public static void LoadHandlers()
        {
            var allMethods = VisionAssembly.GetMethodsWithAttribute<NetPacketHandlerAttribute>().ToList();
            foreach (var pair in allMethods)
            {
                var first = pair.First;
                var second = pair.Second;
                if (Handlers.ContainsKey(first.Command))
                {
                    EngineLog.Warning($"Duplicate message handler found: [{first.Command}], ignoring.");
                    continue;
                }

                var destinations = first.Destinations;
                var theDelegate = Delegate.CreateDelegate(typeof(NetPacketHandlerDelegate<T>), second) as NetPacketHandlerDelegate<T>;
                var both = new Tuple<NetConnectionDestination[], NetPacketHandlerDelegate<T>>(destinations, theDelegate);

                Handlers.Add(first.Command, both);

                var destinationsStr = "";
                for (var index = 0; index < destinations.Length; index++)
                {
                    var dest = destinations[index];
                    destinationsStr += dest.ToMessage();
                    if (index != destinations.Length - 1) destinationsStr += ", ";
                }

                EngineLog.Debug($"Added message handler: (Command: {first.Command}, Destinations: {destinationsStr})");
            }

            EngineLog.Info($"Loaded {Handlers.Count} packet handlers!");

            // var allStructs = VisionAssembly.GetTypesOfBase<NetPacketStruct>().ToList();
            //
            // foreach (var @struct in allStructs)
            // {
            //     var ctor = @struct.GetConstructor(Type.EmptyTypes);
            //     if (ctor == null) continue;
            //     var created = ctor.Invoke(new object[] { });
            //     var command = (NetCommand)@struct.GetDeclaredMethod("GetCommand").Invoke(created, null);
            //     Structs.Add(command, @struct);
            //
            //     Log.Write(LogType.EngineLog, LogLevel.Debug, $"Added struct: (Command: {command}, Struct: {@struct.Name}");
            // }
        }

        public static bool TryGetHandler(NetCommand command, out NetPacketHandlerDelegate<T> handler, out NetConnectionDestination[] destinations)
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
