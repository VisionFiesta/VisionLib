using Vision.Core;
using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.Friend
{
    public class NcFriendRefuseCmd : NetPacketStruct
    {
        public string CharID;

        public override int GetSize() => NameN.Name5Len;

        public override void Read(ReaderStream reader)
        {
            CharID = reader.ReadString(NameN.Name5Len);
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(CharID, NameN.Name5Len);
        }

        protected override NetCommand GetCommand() => NetCommand.NC_FRIEND_REFUSE_CMD;

        public override string ToString() => $"{nameof(CharID)}: {CharID}";
    }
}
