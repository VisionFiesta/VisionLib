using System.Diagnostics.CodeAnalysis;
using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;
using Vision.Game.Structs.Common;

namespace Vision.Game.Structs.Friend
{
    public class NcFriendFindFriendsAck : NetPacketStruct
    {
        public ushort Error;
        public ushort FriendNum;
        public FriendInfo[] Friends;

        public override int GetSize() => 4 + FriendNum * FriendInfo.Size;

        public override void Read(ReaderStream reader)
        {
            Error = reader.ReadUInt16();
            FriendNum = reader.ReadUInt16();

            Friends = new FriendInfo[FriendNum];

            for (var i = 0; i < FriendNum; i++)
            {
                Friends[i] = new FriendInfo();
                Friends[i].Read(reader);
            }
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(Error);
            writer.Write(FriendNum);

            for (var i = 0; i < Friends.Length; i++)
            {
                var friendInfo = Friends[i];
                friendInfo.Write(writer);
            }
        }

        protected override NetCommand GetCommand() => NetCommand.NC_FRIEND_FIND_FRIENDS_ACK;

        protected override bool HasMaximumSize() => false;

        [SuppressMessage("ReSharper", "CoVariantArrayConversion")]
        public override string ToString()
        {
            var friendsString = string.Join(", ", (object[])Friends);
            return $"{nameof(Error)}: {Error}, {nameof(FriendNum)}: {FriendNum}, {nameof(Friends)}: {friendsString}";
        }
    }
}
