using System;

namespace Vision.Core.Networking.Packet
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class NetPacketHandlerAttribute : Attribute
    {
        public readonly NetCommand Command;
        public readonly NetConnectionDestination[] Destinations;

        public NetPacketHandlerAttribute(NetCommand command, params NetConnectionDestination[] destinations)
        {
            Command = command;
            Destinations = destinations;
        }
    }
}
