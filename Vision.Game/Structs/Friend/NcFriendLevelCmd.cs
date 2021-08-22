using Vision.Core;
using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.Friend
{
    public class NcFriendLevelCmd : NetPacketStruct
    {
        public string CharID;
        public byte Level;

        public override int GetSize() => NameN.Name5Len + 1;

        public override void Read(ReaderStream reader)
        {
            CharID = reader.ReadString(NameN.Name5Len);
            Level = reader.ReadByte();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(CharID, NameN.Name5Len);
            writer.Write(Level);
        }

        protected override NetCommand GetCommand() => NetCommand.NC_FRIEND_LEVEL_CMD;

        public override string ToString() => $"{nameof(CharID)}: {CharID}, {nameof(Level)}: {Level}";
    }
}
