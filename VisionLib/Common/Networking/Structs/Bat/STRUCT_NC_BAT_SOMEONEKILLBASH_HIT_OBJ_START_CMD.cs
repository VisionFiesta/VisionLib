using VisionLib.Common.Networking.Packet;

namespace VisionLib.Common.Networking.Structs.Bat
{
    public class STRUCT_NC_BAT_SOMEONEKILLBASH_HIT_OBJ_START_CMD : FiestaNetPacketStruct
    {
        public readonly ushort Caster;
        public readonly STRUCT_NC_BAT_SKILLBASH_HIT_OBJ_START_CMD CastInfo;

        public STRUCT_NC_BAT_SOMEONEKILLBASH_HIT_OBJ_START_CMD(ushort caster, STRUCT_NC_BAT_SKILLBASH_HIT_OBJ_START_CMD castInfo)
        {
            Caster = caster;
            CastInfo = castInfo;
        }

        public override FiestaNetPacket ToPacket()
        {
            var pkt = new FiestaNetPacket(FiestaNetCommand.NC_BAT_SOMEONESKILLBASH_HIT_OBJ_START_CMD);
            WriteToPacket(pkt);
            return pkt;
        }

        public override void WriteToPacket(FiestaNetPacket pkt)
        {
            if (pkt == null) return;
            pkt.Write(Caster);
            CastInfo.WriteToPacket(pkt);
        }
    }
}
