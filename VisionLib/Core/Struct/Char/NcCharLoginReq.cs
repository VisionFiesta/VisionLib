using VisionLib.Common.Networking;
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

        public override FiestaNetCommand GetCommand()
        {
            return FiestaNetCommand.NC_CHAR_LOGIN_REQ;
        }
    }
}
