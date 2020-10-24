using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.Map
{
    public class NcMapLoginCompleteCmd : NetPacketStruct
    {
        public override int GetSize() => 0;

        public override void Read(ReaderStream reader) { }

        public override void Write(WriterStream writer) { }

        public override NetCommand GetCommand() => NetCommand.NC_MAP_LOGINCOMPLETE_CMD;
    }
}
