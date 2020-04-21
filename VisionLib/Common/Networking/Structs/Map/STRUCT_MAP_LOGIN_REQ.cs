using VisionLib.Common.Networking.Packet;

namespace VisionLib.Common.Networking.Structs.Map
{
    public class STRUCT_MAP_LOGIN_REQ : FiestaNetPacketStruct
    {
        public const int SHNHashLen_1_02_274 = 1792;

        public readonly ushort WorldManagerHandle;
        public readonly string CharacterName;
        public readonly string SHNHash;

        public STRUCT_MAP_LOGIN_REQ(ushort worldManagerHandle, string characterName, string shnHash)
        {
            WorldManagerHandle = worldManagerHandle;
            CharacterName = characterName;
            SHNHash = shnHash;
        }

        public STRUCT_MAP_LOGIN_REQ(FiestaNetPacket packet)
        {
            WorldManagerHandle = packet.ReadUInt16();
            CharacterName = packet.ReadString(NameN.Name5Len);
            SHNHash = packet.ReadString(packet.RemainingBytes); // TODO: fail on size?
        }

        public override FiestaNetPacket ToPacket()
        {
            var pkt = new FiestaNetPacket(FiestaNetCommand.NC_MAP_LOGIN_REQ);
            WriteToPacket(pkt);
            return pkt;
        }

        public override void WriteToPacket(FiestaNetPacket pkt)
        {
            if (pkt == null) return;
            pkt.Write(WorldManagerHandle);
            pkt.Write(CharacterName, NameN.Name5Len);
            pkt.Write(SHNHash); // don't limit length here
        }
    }
}
