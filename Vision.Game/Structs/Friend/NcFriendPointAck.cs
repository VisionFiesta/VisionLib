using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.Friend
{
    public class NcFriendPointAck : NetPacketStruct
    {
        public ushort FriendPoints;

        public override int GetSize() => 2;

        public override void Read(ReaderStream reader)
        {
            FriendPoints = reader.ReadUInt16();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(FriendPoints);
        }

        protected override NetCommand GetCommand() => NetCommand.NC_FRIEND_POINT_ACK;

        public override string ToString() => $"{nameof(FriendPoints)}: {FriendPoints}";
    }
}
