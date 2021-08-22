using Vision.Core;
using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.Friend
{
    public class NcFriendSetConfirmAck : NetPacketStruct
    {
        public string ReceiverCharID;
        public string RequesterCharID;
        public bool Accept;

        public override int GetSize() => NameN.Name5Len * 2 + 1;

        public override void Read(ReaderStream reader)
        {
            ReceiverCharID = reader.ReadString(NameN.Name5Len);
            RequesterCharID = reader.ReadString(NameN.Name5Len);
            Accept = reader.ReadBoolean();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(ReceiverCharID);
            writer.Write(RequesterCharID);
            writer.Write(Accept);
        }

        protected override NetCommand GetCommand() => NetCommand.NC_FRIEND_SET_CONFIRM_ACK;
    }
}
