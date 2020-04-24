using VisionLib.Common.Networking.Packet;

namespace VisionLib.Common.Networking.Structs.Bat
{
    public class STRUCT_NC_BAT_SKILLBASH_HIT_OBJ_START_CMD : FiestaNetPacketStruct
    {
        public readonly ushort Skill;
        public readonly ushort TargetObj;
        public readonly ushort Index;

        public STRUCT_NC_BAT_SKILLBASH_HIT_OBJ_START_CMD(ushort skill, ushort targetObj, ushort index)
        {
            Skill = skill;
            TargetObj = targetObj;
            Index = index;
        }

        public override FiestaNetPacket ToPacket()
        {
            var pkt = new FiestaNetPacket(FiestaNetCommand.NC_BAT_SKILLBASH_HIT_OBJ_START_CMD);
            WriteToPacket(pkt);
            return pkt;
        }

        public override void WriteToPacket(FiestaNetPacket pkt)
        {
            if (pkt == null) return;
            pkt.Write(Skill);
            pkt.Write(TargetObj);
            pkt.Write(Index);
        }
    }
}
