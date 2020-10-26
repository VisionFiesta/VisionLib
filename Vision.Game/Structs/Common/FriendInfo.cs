using Vision.Core;
using Vision.Core.Streams;
using Vision.Core.Structs;
using Vision.Game.Characters;

namespace Vision.Game.Structs.Common
{
    public class FriendInfo : AbstractStruct
    {
        public const int Size = FriendDate.Size + NameN.Name5Len + 3 + NameN.Name3Len + 32;

        /*
         * struct PROTO_FRIEND_INFO
           {
           PROTO_FRIEND_DATE logininfo;
           Name5 charid;
           char classid;
           char level;
           char isparty;
           char flag;
           Name3 map;
           char statustitle[32];
           };

         */

        public FriendDate LoginInfo = new FriendDate();
        public string CharID;
        public CharacterClass Class;
        public byte Level;
        public bool IsParty;
        public string Map;
        public byte[] StatusTitle; // 32

        public override int GetSize() => Size;

        public override void Read(ReaderStream reader)
        {
            LoginInfo.Read(reader);

            CharID = reader.ReadString(NameN.Name5Len);
            Class = (CharacterClass)reader.ReadByte();
            Level = reader.ReadByte();
            IsParty = reader.ReadBoolean();
            Map = reader.ReadString(NameN.Name3Len);
            StatusTitle = reader.ReadBytes(32);
        }

        public override void Write(WriterStream writer)
        {
            LoginInfo.Write(writer);

            writer.Write(CharID, NameN.Name5Len);
            writer.Write((byte)Class);
            writer.Write(Level);
            writer.Write(IsParty);
            writer.Write(Map, NameN.Name3Len);
            writer.Write(StatusTitle, 32);
        }

        public override string ToString() => $"{nameof(LoginInfo)}: {LoginInfo}, {nameof(CharID)}: {CharID}, {nameof(Class)}: {Class}, {nameof(Level)}: {Level}, {nameof(IsParty)}: {IsParty}, {nameof(Map)}: {Map}, {nameof(StatusTitle)}: {StatusTitle}";
    }
}
