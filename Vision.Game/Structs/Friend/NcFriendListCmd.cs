using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;
using Vision.Game.Structs.Common;

namespace Vision.Game.Structs.Friend
{
    public class NcFriendListCmd : NetPacketStruct
    {
        public byte FriendNum;
        public FriendInfo[] Friends;

        public override int GetSize() => 1 + Friends.Length * FriendInfo.Size;

        public override void Read(ReaderStream reader)
        {
            FriendNum = reader.ReadByte();

            Friends = new FriendInfo[FriendNum];

            for (var i = 0; i < FriendNum; i++)
            {
                Friends[i] = new FriendInfo();
                Friends[i].Read(reader);
            }
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(FriendNum);

            for (var i = 0; i < Friends.Length; i++)
            {
                var friendInfo = Friends[i];
                friendInfo.Write(writer);
            }
        }

        public override NetCommand GetCommand() => NetCommand.NC_FRIEND_LIST_CMD;

        public override bool HasMaximumSize() => false;

        public override string ToString()
        {
            var friendsString = string.Join(", ", Friends.ToString());
            return $"{nameof(FriendNum)}: {FriendNum}, {nameof(Friends)}: {friendsString}";
        }
    }
}
