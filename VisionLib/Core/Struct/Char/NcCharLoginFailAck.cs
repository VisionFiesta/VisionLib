using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Packet;
using VisionLib.Core.Stream;

namespace VisionLib.Core.Struct.Char
{
    public class NcCharLoginFailAck : NetPacketStruct
    {
        public ushort Error;

        public NcCharLoginFailAck(ushort error)
        {
            Error = error;
        }

        public override FiestaNetPacket ToPacket()
        {
            var pkt = new FiestaNetPacket(FiestaNetCommand.NC_CHAR_LOGINFAIL_ACK);
            Write(pkt.Writer);
            return pkt;
        }

        public override int GetSize()
        {
            return sizeof(ushort);
        }

        public override void Read(ReaderStream reader)
        {
            Error = reader.ReadUInt16();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(Error);
        }
    }
}
