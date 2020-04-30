using Vision.Core.Common;
using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.Act
{
    public class NcActSomeoneWhisperCmd : NetPacketStruct
    {
        public byte ItemLinkDataCount;
        public string SenderName; // Name5
        public byte GMColor;
        public byte MessageLength;
        public string Message;

        public override int GetSize()
        {
            return 23 + MessageLength;
        }

        public override void Read(ReaderStream reader)
        {
            ItemLinkDataCount = reader.ReadByte();
            SenderName = reader.ReadString(NameN.Name5Len);
            GMColor = reader.ReadByte();
            MessageLength = reader.ReadByte();
            Message = reader.ReadString(MessageLength);
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(ItemLinkDataCount);
            writer.Write(SenderName, NameN.Name5Len);
            writer.Write(GMColor);
            writer.Write(MessageLength);
            writer.Write(Message, MessageLength);
        }

        public override FiestaNetCommand GetCommand()
        {
            return FiestaNetCommand.NC_ACT_SOMEONEWHISPER_CMD;
        }
    }
}
