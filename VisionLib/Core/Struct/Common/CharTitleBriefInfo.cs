using VisionLib.Common.Utils;
using VisionLib.Core.Stream;

namespace VisionLib.Core.Struct.Common
{
    public class CharTitleBriefInfo : AbstractStruct
    {
        public byte Type;
        public byte ElementNoValue;
        public ushort MobID;

        public byte ElementNo;
        public byte ElementValue;

        public override int GetSize()
        {
            return 4;
        }

        public override void Read(ReaderStream reader)
        {
            Type = reader.ReadByte();
            ElementNoValue = reader.ReadByte();

            using (var bs = new BitStream())
            {
                bs.Write(ElementNoValue);
                bs.Read(out ElementNo, 0, 6);
                bs.Read(out ElementValue, 0, 2);
            }

            MobID = reader.ReadUInt16();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(Type);

            using (var bs = new BitStream())
            {
                bs.Write(ElementNo, 0, 6);
                bs.Write(ElementValue, 0, 2);
                bs.Read(out ElementNoValue);
            }

            writer.Write(ElementNoValue);

            writer.Write(MobID);
        }
    }
}
