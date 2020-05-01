using Vision.Core;
using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.Map
{
    public class NcMapLoginReq : NetPacketStruct
    {
        public const int SHNHashLen_1_02_276 = 1792;

        public ushort AccountID;
        public string CharacterName;
        public string SHNHash;

        public NcMapLoginReq(ushort accountID, string characterName, string shnHash)
        {
            AccountID = accountID;
            CharacterName = characterName;
            SHNHash = shnHash;
        }

        public override int GetSize()
        {
            return sizeof(ushort) + NameN.Name5Len + SHNHashLen_1_02_276;
        }

        public override void Read(ReaderStream reader)
        {
            AccountID = reader.ReadUInt16();
            CharacterName = reader.ReadString(NameN.Name5Len);
            SHNHash = reader.ReadString(reader.RemainingBytes);
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(AccountID);
            writer.Write(CharacterName, NameN.Name5Len);
            writer.Write(SHNHash, SHNHashLen_1_02_276);
        }

        public override NetCommand GetCommand()
        {
            return NetCommand.NC_MAP_LOGIN_REQ;
        }
    }
}
