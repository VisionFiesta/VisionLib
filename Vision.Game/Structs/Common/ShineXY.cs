using System;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.Common
{
    public class ShineXY : AbstractStruct
    {
        public const int Size = 8;

        public uint X;
        public uint Y;

        public ShineXY()
        {
            X = 0;
            Y = 0;
        }

        public ShineXY(uint x, uint y)
        {
            X = x;
            Y = y;
        }

        public ShineXY(ShineXY pos)
        {
            SetPos(pos);
        }

        public void SetPos(ShineXY pos)
        {
            X = pos.X;
            Y = pos.Y;
        }

        public double GetDistance(ShineXY otherCoords)
        {
            var X2 = otherCoords.X;
            var Y2 = otherCoords.Y;
            return Math.Sqrt(((X2 - X) * (X2 - X)) + (Y2 - Y) * (Y2 - Y));
        }

        public override int GetSize() => Size;

        public override void Read(ReaderStream reader)
        {
            X = reader.ReadUInt32();
            Y = reader.ReadUInt32();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(X);
            writer.Write(Y);
        }

        public override string ToString()
        {
            return $"XY:[{X}, {Y}]";
        }
    }
}
