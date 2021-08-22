using Vision.Core;
using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.Friend
{
    public class NcFriendPartyCmd : NetPacketStruct
    {
        public string CharID;
        public bool IsParty;

        public override int GetSize() => NameN.Name5Len + 1;

        public override void Read(ReaderStream reader)
        {
            CharID = reader.ReadString(NameN.Name5Len);
            IsParty = reader.ReadBoolean();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(CharID, NameN.Name5Len);
            writer.Write(IsParty);
        }

        protected override NetCommand GetCommand() => NetCommand.NC_FRIEND_PARTY_CMD;

        public override string ToString() => $"{nameof(CharID)}: {CharID}, {nameof(IsParty)}: {IsParty}";
    }
}
