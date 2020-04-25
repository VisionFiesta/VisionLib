using VisionLib.Common.Networking.Packet;

namespace VisionLib.Core.Struct
{
    public abstract class NetPacketStruct : AbstractStruct
    {
        public abstract FiestaNetPacket ToPacket();
    }
}
