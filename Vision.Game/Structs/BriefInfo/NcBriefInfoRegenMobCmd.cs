using Vision.Core;
using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;
using Vision.Game.Enums;
using Vision.Game.Structs.Common;

namespace Vision.Game.Structs.BriefInfo
{
    public class NcBriefInfoRegenMobCmd : NetPacketStruct
    {
        /*
        struct __unaligned __declspec(align(1)) PROTO_NC_BRIEFINFO_REGENMOB_CMD
        {
            unsigned __int16 handle;
            char mode;
            unsigned __int16 mobid;
            SHINE_COORD_TYPE coord;
            char flagstate;
                $BA92430D3E249705924BF5BD724F50E2 flag;
            char sAnimation[32];
            char nAnimationLevel;
            char nKQTeamType;
            char bRegenAni;
        };
        */

        public ushort Handle;
        public byte Mode;
        public ushort MobID;
        public ShineXYR Position = new();
        public MobFlag Flags = new();
        public string Animation; //Name32Byte
        public byte AnimationLevel;
        public KQTeamType KQTeam;
        public bool RegenAni;

        public override int GetSize() => 2 + 1 + 2 + ShineXYR.Size + MobFlag.Size + NameN.Name32ByteLen + 1 + 1 + 1;

        public override void Read(ReaderStream reader)
        {
            Handle = reader.ReadUInt16();
            Mode = reader.ReadByte();
            MobID = reader.ReadUInt16();

            Position.Read(reader);
            Flags.Read(reader);

            Animation = reader.ReadString(NameN.Name32ByteLen);
            AnimationLevel = reader.ReadByte();
            KQTeam = (KQTeamType) reader.ReadByte();
            RegenAni = reader.ReadBoolean();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(Handle);
            writer.Write(Mode);
            writer.Write(MobID);
            Position.Write(writer);
            Flags.Write(writer);
            writer.Write(Animation, NameN.Name32ByteLen);
            writer.Write(AnimationLevel);
            writer.Write((byte)KQTeam);
            writer.Write(RegenAni);
        }

        public override NetCommand GetCommand() => NetCommand.NC_BRIEFINFO_REGENMOB_CMD;
    }
}
