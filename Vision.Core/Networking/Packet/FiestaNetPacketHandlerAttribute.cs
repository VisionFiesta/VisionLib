using System;

namespace Vision.Core.Networking.Packet
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class FiestaNetPacketHandlerAttribute : Attribute
    {
        public readonly FiestaNetCommand Command;
        public readonly FiestaNetConnDest[] Destinations;

        public FiestaNetPacketHandlerAttribute(FiestaNetCommand command, params FiestaNetConnDest[] destinations)
        {
            Command = command;
            Destinations = destinations;
        }
    }
}
