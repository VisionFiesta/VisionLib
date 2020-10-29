using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.Char
{
    public class NcCharLoginFailAck : NetPacketStruct
    {
        public const int Size = 2;

        public ushort Error;

        public override int GetSize() => Size;

        public override void Read(ReaderStream reader) => Error = reader.ReadUInt16();

        public override void Write(WriterStream writer) => writer.Write(Error);

        public override NetCommand GetCommand() => NetCommand.NC_CHAR_LOGINFAIL_ACK;
    }
}
