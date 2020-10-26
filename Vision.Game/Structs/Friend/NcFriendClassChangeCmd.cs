using Vision.Core;
using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;
using Vision.Game.Characters;

namespace Vision.Game.Structs.Friend
{
    public class NcFriendClassChangeCmd : NetPacketStruct
    {
        public string CharID;
        public CharacterClass Level;

        public override int GetSize() => NameN.Name5Len + 1;

        public override void Read(ReaderStream reader)
        {
            CharID = reader.ReadString(NameN.Name5Len);
            Level = (CharacterClass) reader.ReadByte();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(CharID, NameN.Name5Len);
            writer.Write((byte) Level);
        }

        public override NetCommand GetCommand() => NetCommand.NC_FRIEND_CLASS_CHANGE_CMD;
    }
}
