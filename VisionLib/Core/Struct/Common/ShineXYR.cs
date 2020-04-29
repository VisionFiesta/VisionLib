﻿using VisionLib.Core.Stream;

namespace VisionLib.Core.Struct.Common
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
    }
}
