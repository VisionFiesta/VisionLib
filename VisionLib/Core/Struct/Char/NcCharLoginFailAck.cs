using VisionLib.Common.Networking;
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

        public override FiestaNetCommand GetCommand()
        {
            return FiestaNetCommand.NC_CHAR_LOGINFAIL_ACK;
        }
    }
}
