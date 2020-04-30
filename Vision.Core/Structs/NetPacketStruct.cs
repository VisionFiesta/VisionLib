using Vision.Core.Networking;
using Vision.Core.Networking.Packet;

namespace Vision.Core.Structs
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
