using VisionLib.Common.Networking;
using VisionLib.Core.Stream;

namespace VisionLib.Core.Struct.Misc
{
    public class NcMiscGameTimeReq : NetPacketStruct
    {
        public override int GetSize() => 0;

        public override void Read(ReaderStream reader) { }

        public override void Write(WriterStream writer) { }

        public override FiestaNetCommand GetCommand() => FiestaNetCommand.NC_MISC_GAMETIME_REQ;
    }
}
