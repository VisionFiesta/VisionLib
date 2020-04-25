using VisionLib.Common;
using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Packet;
using VisionLib.Core.Stream;

namespace VisionLib.Core.Struct.Map
{
    public class NcMapLoginReq : NetPacketStruct
    {
        public const int SHNHashLen_1_02_274 = 1792;

        public ushort WorldManagerHandle;
        public string CharacterName;
        public string SHNHash;

        public NcMapLoginReq(ushort worldManagerHandle, string characterName, string shnHash)
        {
            WorldManagerHandle = worldManagerHandle;
            CharacterName = characterName;
            SHNHash = shnHash;
        }

        public override FiestaNetPacket ToPacket()
        {
            var pkt = new FiestaNetPacket(FiestaNetCommand.NC_MAP_LOGIN_REQ);
            Write(pkt.Writer);
            return pkt;
        }

        public override int GetSize()
        {
            return sizeof(ushort) + NameN.Name5Len + SHNHashLen_1_02_274;
        }

        public override void Read(ReaderStream reader)
        {
            WorldManagerHandle = reader.ReadUInt16();
            CharacterName = reader.ReadString(NameN.Name5Len);
            SHNHash = reader.ReadString(reader.RemainingBytes);
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(WorldManagerHandle);
            writer.Write(CharacterName, NameN.Name5Len);
            writer.Write(SHNHash, SHNHashLen_1_02_274);
        }
    }
}
