using Vision.Core.Streams;

namespace Vision.Game.Structs.Common
{
    public class ShineXYR : ShineXY
    {
        public new const int Size = 1 + ShineXY.Size;

        public byte Rotation;

        public override int GetSize() => Size;

        public override void Read(ReaderStream reader)
        {
            base.Read(reader);
            Rotation = reader.ReadByte();
        }

        public override void Write(WriterStream writer)
        {
            base.Write(writer);
            writer.Write(Rotation);
        }

        public override string ToString()
        {
            return $"XYR: [{X}, {Y}, {Rotation}]";
        }
    }
}
