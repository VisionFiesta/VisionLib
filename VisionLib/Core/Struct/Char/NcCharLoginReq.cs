using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Packet;
using VisionLib.Core.Stream;

namespace VisionLib.Core.Struct.Char
{
    public class NcCharLoginReq : NetPacketStruct
    {
        public byte CharSlot { get; private set; }

        public NcCharLoginReq(byte charSlot)
        {
            CharSlot = charSlot;
        }

        public override FiestaNetPacket ToPacket()
        {
            var pkt = new FiestaNetPacket(FiestaNetCommand.NC_CHAR_LOGIN_REQ);
            Write(pkt.Writer);
            return pkt;
        }

        public override int GetSize()
        {
            return sizeof(byte);
        }

        public override void Read(ReaderStream reader)
        {
            CharSlot = reader.ReadByte();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(CharSlot);
        }
    }
}
