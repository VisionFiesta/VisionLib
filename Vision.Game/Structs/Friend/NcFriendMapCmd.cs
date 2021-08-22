using Vision.Core;
using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.Friend
{
    public class NcFriendMapCmd : NetPacketStruct
    {
        public string CharID;
        public string Map;

        public override int GetSize() => NameN.Name5Len + NameN.Name3Len;

        public override void Read(ReaderStream reader)
        {
            CharID = reader.ReadString(NameN.Name5Len);
            Map = reader.ReadString(NameN.Name3Len);
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(CharID, NameN.Name5Len);
            writer.Write(Map, NameN.Name3Len);
        }

        protected override NetCommand GetCommand() => NetCommand.NC_FRIEND_MAP_CMD;

        public override string ToString() => $"{nameof(CharID)}: {CharID}, {nameof(Map)}: {Map}";
    }
}
