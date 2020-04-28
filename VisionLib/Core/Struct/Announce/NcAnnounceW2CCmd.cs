﻿using VisionLib.Common.Enums;
using VisionLib.Common.Networking;
using VisionLib.Core.Stream;

namespace VisionLib.Core.Struct.Announce
{
    public class NcAnnounceW2CCmd : NetPacketStruct
    {
        public AnnounceType AnnounceType;
        public byte MessageLength;
        public string Message;
        
        public override int GetSize()
        {
            return sizeof(byte) * 2 + MessageLength;
        }

        public override void Read(ReaderStream reader)
        {
            AnnounceType = (AnnounceType)reader.ReadByte();
            MessageLength = reader.ReadByte();
            Message = reader.ReadString(MessageLength);
        }

        public override void Write(WriterStream writer)
        {
            writer.Write((byte)AnnounceType);
            writer.Write(MessageLength);
            writer.Write(Message, MessageLength);
        }

        public override FiestaNetCommand GetCommand()
        {
            return FiestaNetCommand.NC_ANNOUNCE_W2C_CMD;
        }
    }
}
