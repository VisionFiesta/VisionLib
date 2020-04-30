using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.Common
{
    public class ShortCutData : AbstractStruct
    {
        public byte SlotNo; // +1
        public ushort CodeNo; // +2
        public int Value; // +4

        public override int GetSize()
        {
            return 7;
        }

        public override void Read(ReaderStream reader)
        {
            SlotNo = reader.ReadByte();
            CodeNo = reader.ReadUInt16();
            Value = reader.ReadInt32();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(SlotNo);
            writer.Write(CodeNo);
            writer.Write(Value);
        }
    }
}
