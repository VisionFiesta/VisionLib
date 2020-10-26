using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;
using Vision.Game.Structs.Common;

namespace Vision.Game.Structs.BriefInfo
{
    public class NcBriefInfoDropedItemCmd : NetPacketStruct
    {
        public const int Size = 4 + ShineXY.Size + 2;

        public ushort Handle;
        public ushort Item;
        public ShineXY Position = new ShineXY();
        public ushort DropMobHandle;
        // $2B3919EF3E87878EEF5310356B483899 attr; // unk size

        public override int GetSize() => Size; // + unk

        public override void Read(ReaderStream reader)
        {
            Handle = reader.ReadUInt16();
            Item = reader.ReadUInt16();

            Position.Read(reader);

            DropMobHandle = reader.ReadUInt16();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(Handle);
            writer.Write(Item);

            Position.Write(writer);

            writer.Write(DropMobHandle);
        }

        public override NetCommand GetCommand() => NetCommand.NC_BRIEFINFO_DROPEDITEM_CMD;

        public override string ToString() => $"{nameof(Handle)}: {Handle}, {nameof(Item)}: {Item}, {nameof(Position)}: {Position}, {nameof(DropMobHandle)}: {DropMobHandle}";
    }
}
