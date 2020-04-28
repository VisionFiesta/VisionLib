using VisionLib.Core.Stream;

namespace VisionLib.Core.Struct.Common
{
    public class PlayerQuestDoneInfo : AbstractStruct
    {
        public ushort ID;
        public long EndTime;

        public override int GetSize()
        {
            return 10;
        }

        public override void Read(ReaderStream reader)
        {
            ID = reader.ReadUInt16();
            EndTime = reader.ReadInt64();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(ID);
            writer.Write(EndTime);
        }
    }
}
