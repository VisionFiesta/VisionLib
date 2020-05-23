﻿using Vision.Core.Streams;
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
    }
}