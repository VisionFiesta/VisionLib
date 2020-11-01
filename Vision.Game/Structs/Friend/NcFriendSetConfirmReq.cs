using Vision.Core;
using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.Friend
{
    public class NcFriendSetConfirmReq : NetPacketStruct
    {
        public string ReceiverCharID;
        public string RequesterCharID;

        public override int GetSize() => NameN.Name5Len * 2;

        public override void Read(ReaderStream reader)
        {
            ReceiverCharID = reader.ReadString(NameN.Name5Len);
            RequesterCharID = reader.ReadString(NameN.Name5Len);
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(ReceiverCharID);
            writer.Write(RequesterCharID);
        }

        public override NetCommand GetCommand() => NetCommand.NC_FRIEND_SET_CONFIRM_REQ;
    }
}
