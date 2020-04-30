using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.Char
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

        public override NetCommand GetCommand()
        {
            return NetCommand.NC_CHAR_LOGINFAIL_ACK;
        }
    }
}
