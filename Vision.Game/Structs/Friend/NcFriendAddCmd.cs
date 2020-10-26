using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;
using Vision.Game.Structs.Common;

namespace Vision.Game.Structs.Friend
{
    public class NcFriendAddCmd : NetPacketStruct
    {
        public FriendInfo FriendInfo = new FriendInfo();

        public override int GetSize() => FriendInfo.Size;

        public override void Read(ReaderStream reader) => FriendInfo.Read(reader);

        public override void Write(WriterStream writer) => FriendInfo.Write(writer);

        public override NetCommand GetCommand() => NetCommand.NC_FRIEND_ADD_CMD;

        public override string ToString() => $"{nameof(FriendInfo)}: {FriendInfo}";
    }
}
