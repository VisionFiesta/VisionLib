using VisionLib.Common.Networking;
using VisionLib.Core.Stream;

namespace VisionLib.Core.Struct.Bat
{
    public class NcBatSomeoneKillBashHitObjStartCmd : NetPacketStruct
    {
        public ushort Caster { get; private set; }
        public NcBatSkillBashHitObjStartCmd CastInfo { get; private set; }

        public NcBatSomeoneKillBashHitObjStartCmd(ushort caster, NcBatSkillBashHitObjStartCmd castInfo)
        {
            Caster = caster;
            CastInfo = castInfo;
        }

        public override int GetSize()
        {
            return sizeof(ushort) * 4;
        }

        public override void Read(ReaderStream reader)
        {
            Caster = reader.ReadUInt16();
            CastInfo = new NcBatSkillBashHitObjStartCmd();
            CastInfo.Read(reader);
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(Caster);
            CastInfo.Write(writer);
        }

        public override FiestaNetCommand GetCommand()
        {
            return FiestaNetCommand.NC_BAT_SOMEONESKILLBASH_HIT_OBJ_START_CMD;
        }
    }
}
