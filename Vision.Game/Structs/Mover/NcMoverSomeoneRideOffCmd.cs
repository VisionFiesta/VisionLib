using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.Mover
{
    public class NcMoverSomeoneRideOffCmd : NetPacketStruct
    {
        public ushort PlayerHandle;

        public override int GetSize() => 2;

        public override void Read(ReaderStream reader) => PlayerHandle = reader.ReadUInt16();

        public override void Write(WriterStream writer) => writer.Write(PlayerHandle);

        public override NetCommand GetCommand() => NetCommand.NC_ACT_SOMEONERIDE_OFF_CMD;
    }
}
