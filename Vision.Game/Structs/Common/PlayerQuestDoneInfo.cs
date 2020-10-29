using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.Common
{
    public class PlayerQuestDoneInfo : AbstractStruct
    {
        public const int Size = 10;

        public ushort ID;
        public long EndTime;

        public override int GetSize() => Size;

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
