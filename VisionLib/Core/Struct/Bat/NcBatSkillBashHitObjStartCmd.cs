using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Packet;
using VisionLib.Core.Stream;

namespace VisionLib.Core.Struct.Bat
{
    public class NcBatSkillBashHitObjStartCmd : NetPacketStruct
    {
        public ushort Skill { get; private set; }
        public ushort TargetObj { get; private set; }
        public ushort Index { get; private set; }

        public NcBatSkillBashHitObjStartCmd() { }

        public NcBatSkillBashHitObjStartCmd(ushort skill, ushort targetObj, ushort index)
        {
            Skill = skill;
            TargetObj = targetObj;
            Index = index;
        }

        public override FiestaNetPacket ToPacket()
        {
            var pkt = new FiestaNetPacket(FiestaNetCommand.NC_BAT_SKILLBASH_HIT_OBJ_START_CMD);
            Write(pkt.Writer);
            return pkt;
        }

        public override int GetSize()
        {
            return sizeof(ushort) * 3;
        }

        public override void Read(ReaderStream reader)
        {
            Skill = reader.ReadUInt16();
            TargetObj = reader.ReadUInt16();
            Index = reader.ReadUInt16();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(Skill);
            writer.Write(TargetObj);
            writer.Write(Index);
        }
    }
}
