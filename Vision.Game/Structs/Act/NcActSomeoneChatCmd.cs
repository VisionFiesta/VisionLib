using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.Act
{
    public class NcActSomeoneChatCmd : NetPacketStruct
    {
        public byte ItemLinkDataCount;
        public ushort Handle;
        public byte Length;
        public byte GMColorChatWin;
        public byte ChatFontColorID;
        public byte ChatBalloonColorID;
        public string Message;

        public override int GetSize()
        {
            return 7 + Length * 1;
        }

        public override void Read(ReaderStream reader)
        {
            ItemLinkDataCount = reader.ReadByte();
            Handle = reader.ReadUInt16();
            Length = reader.ReadByte();

            GMColorChatWin = reader.ReadByte(); // TODO: bitpacking/unpacking

            ChatFontColorID = reader.ReadByte();
            ChatBalloonColorID = reader.ReadByte();

            Message = reader.ReadString(Length);
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(ItemLinkDataCount);
            writer.Write(Handle);
            writer.Write(Length);
            writer.Write(GMColorChatWin);
            writer.Write(ChatFontColorID);
            writer.Write(ChatBalloonColorID);
            writer.Write(Message, Length);
        }

        public override NetCommand GetCommand()
        {
            return NetCommand.NC_ACT_SOMEONECHAT_CMD;
        }
    }
}
