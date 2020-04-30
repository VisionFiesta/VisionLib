using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.Common
{
    public class KeyMapData : AbstractStruct
    {
        public ushort FunctionNo; // +2
        public byte ExtendKey; // +1
        public byte ASCIICode; // +1

        public override int GetSize()
        {
            return 4;
        }

        public override void Read(ReaderStream reader)
        {
            FunctionNo = reader.ReadUInt16();
            ExtendKey = reader.ReadByte();
            ASCIICode = reader.ReadByte();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(FunctionNo);
            writer.Write(ExtendKey);
            writer.Write(ASCIICode);
        }
    }
}
