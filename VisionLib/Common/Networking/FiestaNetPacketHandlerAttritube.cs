using System;

namespace VisionLib.Common.Networking
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class FiestaNetPacketHandlerAttritube : Attribute
    {
        public readonly FiestaNetCommand Command;

        public FiestaNetPacketHandlerAttritube(FiestaNetCommand command)
        {
            Command = command;
        }
    }
}
