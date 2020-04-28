using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Packet;

namespace VisionLib.Core.Struct
{
    public abstract class NetPacketStruct : AbstractStruct
    {
        public abstract FiestaNetCommand GetCommand();

        public void Read(FiestaNetPacket packet)
        {
            Read(packet.Reader);
        }

        public void Send(FiestaNetConnection connection)
        {
            var pkt = new FiestaNetPacket(GetCommand());
            Write(pkt.Writer);
            pkt.Send(connection);
        }
    }
}
