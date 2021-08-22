using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.BriefInfo
{
    public class NcBriefInfoChangeDecorateCmd : NetPacketStruct
    {
        public ushort Handle;
        public ushort Item;
        public byte CharSlotNum;

        public override int GetSize() => 5;

        public override void Read(ReaderStream reader)
        {
            Handle = reader.ReadUInt16();
            Item = reader.ReadUInt16();
            CharSlotNum = reader.ReadByte();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(Handle);
            writer.Write(Item);
            writer.Write(CharSlotNum);
        }

        protected override NetCommand GetCommand() => NetCommand.NC_BRIEFINFO_CHANGEDECORATE_CMD;

        public override string ToString() => $"{nameof(Handle)}: {Handle}, {nameof(Item)}: {Item}, {nameof(CharSlotNum)}: {CharSlotNum}";
    }
}
