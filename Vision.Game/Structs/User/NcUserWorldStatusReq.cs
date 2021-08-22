using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.User
{
    public class NcUserWorldStatusReq : NetPacketStruct
    {
        public override int GetSize() => 0;

        public override void Read(ReaderStream reader) { }

        public override void Write(WriterStream writer) { }

        protected override NetCommand GetCommand() => NetCommand.NC_USER_WORLD_STATUS_REQ;
    }
}
