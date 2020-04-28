﻿using VisionLib.Core.Stream;

namespace VisionLib.Core.Struct.Common
{
    public class CharParamChangeCmd : AbstractStruct
    {
        public byte StatID;
        public uint StatType;

        public override int GetSize()
        {
            return 5;
        }

        public override void Read(ReaderStream reader)
        {
            StatID = reader.ReadByte();
            StatType = reader.ReadUInt32();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(StatID);
            writer.Write(StatType);
        }
    }
}
