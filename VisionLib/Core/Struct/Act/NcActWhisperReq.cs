﻿using VisionLib.Common;
using VisionLib.Common.Extensions;
using VisionLib.Common.Networking;
using VisionLib.Core.Stream;

namespace VisionLib.Core.Struct.Act
{
    public class NcActWhisperReq : NetPacketStruct
    {
        public byte ItemLinkDataCount;
        public string ReceiverName; // Name5
        public byte MessageLength;
        public string Message;

        public NcActWhisperReq() { }

        public NcActWhisperReq(string receiverName, string message)
        {
            message.Truncate(sizeof(byte));

            ReceiverName = receiverName;
            MessageLength = (byte) message.Length;
            Message = message;
        }

        public override int GetSize()
        {
            return 22 + MessageLength;
        }

        public override void Read(ReaderStream reader)
        {
            ItemLinkDataCount = reader.ReadByte();
            ReceiverName = reader.ReadString(NameN.Name5Len);
            MessageLength = reader.ReadByte();
            Message = reader.ReadString(MessageLength);
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(ItemLinkDataCount);
            writer.Write(ReceiverName, NameN.Name5Len);
            writer.Write(MessageLength);
            writer.Write(Message, MessageLength);
        }

        public override FiestaNetCommand GetCommand()
        {
            return FiestaNetCommand.NC_ACT_WHISPER_REQ;
        }
    }
}