﻿using VisionLib.Common.Networking;
using VisionLib.Core.Stream;

namespace VisionLib.Core.Struct.User
{
    public class NcUserWorldSelectReq : NetPacketStruct
    {
        public byte WorldID { get; private set; }

        public NcUserWorldSelectReq(byte worldID)
        {
            WorldID = worldID;
        }

        public override int GetSize()
        {
            return sizeof(byte);
        }

        public override void Read(ReaderStream reader)
        {
            WorldID = reader.ReadByte();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(WorldID);
        }

        public override FiestaNetCommand GetCommand()
        {
            return FiestaNetCommand.NC_USER_WORLDSELECT_REQ;
        }
    }
}
