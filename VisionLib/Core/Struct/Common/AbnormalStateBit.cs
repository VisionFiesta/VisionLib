using VisionLib.Core.Stream;

namespace VisionLib.Core.Struct.Common
{
    public class AbnormalStateBit : AbstractStruct
    {
        public const int Size = 111;

        public byte[] State { get; protected set; } = new byte[Size];

        public override int GetSize() => Size;

        public override void Read(ReaderStream reader)
        {
            State = reader.ReadBytes(Size);
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(State, Size);
        }
    }
}
