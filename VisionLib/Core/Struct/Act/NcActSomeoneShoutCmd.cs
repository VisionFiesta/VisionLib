using VisionLib.Common;
using VisionLib.Common.Networking;
using VisionLib.Core.Stream;

namespace VisionLib.Core.Struct.Act
{
    public class NcActSomeoneShoutCmd : NetPacketStruct
    {
        public byte ItemLinkDataCount;
        public string SenderName; // Name5
        public byte MobID;
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
            MobID = reader.ReadByte();
            MessageLength = reader.ReadByte();
            Message = reader.ReadString(MessageLength);
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(ItemLinkDataCount);
            writer.Write(SenderName, NameN.Name5Len);
            writer.Write(MobID);
            writer.Write(MessageLength);
            writer.Write(Message, MessageLength);
        }

        public override FiestaNetCommand GetCommand()
        {
            return FiestaNetCommand.NC_ACT_SOMEONESHOUT_CMD;
        }
    }
}
