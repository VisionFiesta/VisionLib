using VisionLib.Core.Stream;

namespace VisionLib.Core.Struct.Common
{
    public class StopEmoticonDescript : AbstractStruct
    {
        public byte EmoticonID;
        public ushort EmoticonFrame;

        public override int GetSize()
        {
            return 3;
        }

        public override void Read(ReaderStream reader)
        {
            EmoticonID = reader.ReadByte();
            EmoticonFrame = reader.ReadUInt16();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(EmoticonID);
            writer.Write(EmoticonFrame);
        }
    }
}
