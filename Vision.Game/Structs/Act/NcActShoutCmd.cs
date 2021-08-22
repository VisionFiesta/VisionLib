using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.Act
{
    public class NcActShoutCmd : NetPacketStruct
    {
        public byte ItemLinkDataCount;
        public byte MessageLength;
        public string Message;
        
        public NcActShoutCmd() { }

        public NcActShoutCmd(string message)
        {
            MessageLength = (byte) message.Length;
            Message = message;
        }

        public override int GetSize() => 2 * MessageLength;

        public override void Read(ReaderStream reader)
        {
            ItemLinkDataCount = reader.ReadByte();
            MessageLength = reader.ReadByte();
            Message = reader.ReadString(MessageLength);
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(ItemLinkDataCount);
            writer.Write(MessageLength);
            writer.Write(Message, MessageLength);
        }

        protected override NetCommand GetCommand() => NetCommand.NC_ACT_SHOUT_CMD;
    }
}
