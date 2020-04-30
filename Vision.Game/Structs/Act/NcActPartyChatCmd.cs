using Vision.Core.Common;
using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.Act
{
    public class NcActPartyChatCmd : NetPacketStruct
    {
        public string SenderName; // Name5
        public NcActPartyChatReq Chat;

        public override int GetSize()
        {
            return NameN.Name5Len + Chat.GetSize();
        }

        public override void Read(ReaderStream reader)
        {
            SenderName = reader.ReadString(NameN.Name5Len);

            Chat = new NcActPartyChatReq();
            Chat.Read(reader);
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(SenderName, NameN.Name5Len);
            Chat.Write(writer);
        }

        public override FiestaNetCommand GetCommand()
        {
            return FiestaNetCommand.NC_ACT_PARTYCHAT_CMD;
        }
    }
}
