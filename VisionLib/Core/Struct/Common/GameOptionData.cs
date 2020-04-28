using VisionLib.Core.Stream;

namespace VisionLib.Core.Struct.Common
{
    public class GameOptionData : AbstractStruct
    {
        public ushort OptionNo;
        public byte Value;

        public GameOptionData() { }

        public GameOptionData(ushort optionNo, byte value)
        {
            OptionNo = optionNo;
            Value = value;
        }

        public override int GetSize()
        {
            return sizeof(ushort) + sizeof(byte);
        }

        public override void Read(ReaderStream reader)
        {
            OptionNo = reader.ReadUInt16();
            Value = reader.ReadByte();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(OptionNo);
            writer.Write(Value);
        }
    }
}
