using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.Friend
{
    public class NcFriendUseFriendPointAck : NetPacketStruct
    {
        public ushort Error;
        public ushort FriendPoints;
        public ushort ItemID;
        public byte NumOfItem;


        public override int GetSize() => 7;

        public override void Read(ReaderStream reader)
        {
            Error = reader.ReadUInt16();
            FriendPoints = reader.ReadUInt16();
            ItemID = reader.ReadUInt16();
            NumOfItem = reader.ReadByte();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(Error);
            writer.Write(FriendPoints);
            writer.Write(ItemID);
            writer.Write(NumOfItem);
        }

        protected override NetCommand GetCommand() => NetCommand.NC_FRIEND_UES_FRIEND_POINT_ACK;

        public override string ToString() => $"{nameof(Error)}: {Error}, {nameof(FriendPoints)}: {FriendPoints}, {nameof(ItemID)}: {ItemID}, {nameof(NumOfItem)}: {NumOfItem}";
    }
}
