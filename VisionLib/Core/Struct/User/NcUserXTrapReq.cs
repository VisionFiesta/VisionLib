using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Packet;
using VisionLib.Core.Stream;

namespace VisionLib.Core.Struct.User
{
    public class NcUserXTrapReq : NetPacketStruct
    {
        public byte XTrapHashLength { get; private set; }
        public string XTrapVersionHash { get; private set; }

        public NcUserXTrapReq(byte xTrapHashLength, string xTrapVersionHash)
        {
            XTrapHashLength = xTrapHashLength;
            XTrapVersionHash = xTrapVersionHash;
        }

        public override FiestaNetPacket ToPacket()
        {
            var pkt = new FiestaNetPacket(FiestaNetCommand.NC_USER_XTRAP_REQ);
            Write(pkt.Writer);
            return pkt;
        }

        public override int GetSize()
        {
            return sizeof(byte) + XTrapHashLength;
        }

        public override void Read(ReaderStream reader)
        {
            XTrapHashLength = reader.ReadByte();
            XTrapVersionHash = reader.ReadString(XTrapHashLength);
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(XTrapHashLength);
            writer.Write(XTrapVersionHash, XTrapHashLength);
        }
    }
}
