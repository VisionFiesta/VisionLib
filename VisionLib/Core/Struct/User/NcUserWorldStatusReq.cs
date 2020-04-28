using VisionLib.Common.Networking;
using VisionLib.Core.Stream;

namespace VisionLib.Core.Struct.User
{
    public class NcUserWorldStatusReq : NetPacketStruct
    {
        public override int GetSize() => 0;

        public override void Read(ReaderStream reader) { }

        public override void Write(WriterStream writer) { }

        public override FiestaNetCommand GetCommand() => FiestaNetCommand.NC_USER_WORLD_STATUS_REQ;
    }
}
