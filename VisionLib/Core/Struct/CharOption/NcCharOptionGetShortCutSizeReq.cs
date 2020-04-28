using System;
using System.Collections.Generic;
using System.Text;
using VisionLib.Common.Networking;
using VisionLib.Core.Stream;

namespace VisionLib.Core.Struct.CharOption
{
    public class NcCharOptionGetShortCutSizeReq : NetPacketStruct
    {
        public override int GetSize() => 0;

        public override void Read(ReaderStream reader) { }

        public override void Write(WriterStream writer) { }

        public override FiestaNetCommand GetCommand() => FiestaNetCommand.NC_CHAR_OPTION_DB_GET_SHORTCUTSIZE_REQ;
    }
}
