﻿using System.Diagnostics.CodeAnalysis;
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

        public override int GetSize()
        {
            int len = Friends?.Length ?? 0;
            return 1 + len * FriendInfo.Size;
        }

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

        [SuppressMessage("ReSharper", "CoVariantArrayConversion")]
        public override string ToString()
        {
            var friendsString = string.Join(", ", (object[])Friends);
            return $"{nameof(FriendNum)}: {FriendNum}, {nameof(Friends)}: {friendsString}";
        }
    }
}
