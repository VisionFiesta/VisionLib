using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.BriefInfo
{
    public class NcBriefInfoUnequipCmd : NetPacketStruct
    {
        public ushort Handle;
        public byte Slot;

        public override int GetSize() => 3;

        public override void Read(ReaderStream reader)
        {
            Handle = reader.ReadUInt16();
            Slot = reader.ReadByte();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(Handle);
            writer.Write(Slot);
        }

        public override NetCommand GetCommand() => NetCommand.NC_BRIEFINFO_UNEQUIP_CMD;

        public override string ToString()
        {
            return $"{nameof(Handle)}: {Handle}, {nameof(Slot)}: {Slot}";
        }
    }
}
