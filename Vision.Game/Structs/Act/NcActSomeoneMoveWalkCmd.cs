﻿using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;
using Vision.Game.Structs.Common;

namespace Vision.Game.Structs.Act
{
    public class NcActSomeoneMoveWalkCmd : NetPacketStruct
    {
        public ushort Handle;
        public ShineXY FromPosition = new();
        public ShineXY ToPosition = new();
        public ushort Speed;
        public ushort MoveAttributes;

        public override int GetSize() => 22;

        public override void Read(ReaderStream reader)
        {
            Handle = reader.ReadUInt16();

            FromPosition.Read(reader);
            ToPosition.Read(reader);

            Speed = reader.ReadUInt16();
            MoveAttributes = reader.ReadUInt16();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(Handle);

            FromPosition.Write(writer);
            ToPosition.Write(writer);

            writer.Write(Speed);
            writer.Write(MoveAttributes);
        }

        protected override NetCommand GetCommand() => NetCommand.NC_ACT_SOMEONEMOVEWALK_CMD;
    }
}
