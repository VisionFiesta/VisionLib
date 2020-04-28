using VisionLib.Core.Stream;

namespace VisionLib.Core.Struct.Common
{
    public class ShineXYR : ShineXY
    {
        public double Rotation;

        public override int GetSize()
        {
            return 16;
        }

        public override void Read(ReaderStream reader)
        {
            base.Read(reader);
            Rotation = reader.ReadDouble();
        }

        public override void Write(WriterStream writer)
        {
            base.Write(writer);
            writer.Write(Rotation);
        }
    }
}
