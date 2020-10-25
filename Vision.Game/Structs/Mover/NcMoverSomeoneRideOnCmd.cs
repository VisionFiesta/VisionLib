using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.Mover
{
    class NcMoverSomeoneRideOnCmd : NetPacketStruct
    {
        public ushort PlayerHandle;
        public ushort MoverHandle;
        public byte MoverSlot;
        public byte MoverGrade;

        public override int GetSize() => 6;

        public override void Read(ReaderStream reader)
        {
            PlayerHandle = reader.ReadUInt16();
            MoverHandle = reader.ReadUInt16();
            MoverSlot = reader.ReadByte();
            MoverGrade = reader.ReadByte();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(PlayerHandle);
            writer.Write(MoverHandle);
            writer.Write(MoverSlot);
            writer.Write(MoverGrade);
        }

        public override NetCommand GetCommand() => NetCommand.NC_MOVER_SOMEONE_RIDE_ON_CMD;
    }
}
