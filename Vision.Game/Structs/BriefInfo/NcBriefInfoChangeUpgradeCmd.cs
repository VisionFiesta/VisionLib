using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.BriefInfo
{
    public class NcBriefInfoChangeUpgradeCmd : NetPacketStruct
    {
        public const int Size = 6;

        public ushort Handle;
        public ushort Item;
        public byte Upgrade;
        public byte SlotNum;

        public override int GetSize() => Size;

        public override void Read(ReaderStream reader)
        {
            Handle = reader.ReadUInt16();
            Item = reader.ReadUInt16();
            Upgrade = reader.ReadByte();
            SlotNum = reader.ReadByte();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(Handle);
            writer.Write(Item);
            writer.Write(Upgrade);
            writer.Write(SlotNum);
        }

        protected override NetCommand GetCommand() => NetCommand.NC_BRIEFINFO_CHANGEUPGRADE_CMD;

        public override string ToString()
        {
            return $"{nameof(Handle)}: {Handle}, {nameof(Item)}: {Item}, {nameof(Upgrade)}: {Upgrade}, {nameof(SlotNum)}: {SlotNum}";
        }
    }
}
