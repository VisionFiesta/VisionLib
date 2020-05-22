using Vision.Core.Streams;
using Vision.Core.Structs;
using Vision.Core.Utils;

namespace Vision.Game.Structs.Common
{
    public class CharTitleInfo : AbstractStruct
    {
        public byte Type;
        public byte ElementNoValue;

        public byte ElementNo;
        public byte ElementValue;

        public override int GetSize()
        {
            return 2;
        }

        public override void Read(ReaderStream reader)
        {
            Type = reader.ReadByte();
            ElementNoValue = reader.ReadByte();

            using var bs = new BitStream();
            bs.Write(ElementNoValue);
            bs.Read(out ElementNo, 0, 6);
            bs.Read(out ElementValue, 0, 2);
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
        }
    }
}