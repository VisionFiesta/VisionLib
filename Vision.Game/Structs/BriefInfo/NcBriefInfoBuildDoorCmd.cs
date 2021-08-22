using Vision.Core;
using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;
using Vision.Game.Structs.Common;

namespace Vision.Game.Structs.BriefInfo
{
    public class NcBriefInfoBuildDoorCmd : NetPacketStruct
    {
        public const int Size = 2 + 2 + ShineXYR.Size + 1 + NameN.Name8Len + 2;

        public ushort Handle;
        public ushort MobID;
        public ShineXYR Coordinates = new();
        public byte DoorState;
        public string BlockIndex;
        public ushort Scale;

        public override int GetSize() => Size;

        public override void Read(ReaderStream reader)
        {
            Handle = reader.ReadUInt16();
            MobID = reader.ReadUInt16();

            Coordinates.Read(reader);

            DoorState = reader.ReadByte();
            BlockIndex = reader.ReadString(NameN.Name8Len);
            Scale = reader.ReadUInt16();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(Handle);
            writer.Write(MobID);

            Coordinates.Write(writer);

            writer.Write(DoorState);
            writer.Write(BlockIndex);
            writer.Write(Scale);
        }

        protected override NetCommand GetCommand() => NetCommand.NC_BRIEFINFO_BUILDDOOR_CMD;
    }
}
