using Vision.Core;
using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.Friend
{
    public class NcFriendSetAck : NetPacketStruct
    {
        public string CharID;
        public string FriendID;
        public ushort Error;

        public override int GetSize() => NameN.Name5Len * 2 + 2;

        public override void Read(ReaderStream reader)
        {
            CharID = reader.ReadString(NameN.Name5Len);
            FriendID = reader.ReadString(NameN.Name5Len);
            Error = reader.ReadUInt16();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(CharID, NameN.Name5Len);
            writer.Write(FriendID, NameN.Name5Len);
            writer.Write(Error);
        }

        public override NetCommand GetCommand() => NetCommand.NC_FRIEND_SET_ACK;

        public override string ToString() => $"{nameof(CharID)}: {CharID}, {nameof(FriendID)}: {FriendID}, {nameof(Error)}: {Error}";
    }
}
