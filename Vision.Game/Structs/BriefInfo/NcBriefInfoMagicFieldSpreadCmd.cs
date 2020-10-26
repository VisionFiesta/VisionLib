using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;
using Vision.Game.Structs.Common;

namespace Vision.Game.Structs.BriefInfo
{
    public class NcBriefInfoMagicFieldSpreadCmd : NetPacketStruct
    {
        public const int Size = 8 + ShineXY.Size;

        public ushort Handle;
        public ushort Caster;
        public ushort SkillID;
        public ShineXY Position = new ShineXY();
        public ushort Radius;

        public override int GetSize() => Size;

        public override void Read(ReaderStream reader)
        {
            Handle = reader.ReadUInt16();
            Caster = reader.ReadUInt16();
            SkillID = reader.ReadUInt16();

            Position.Read(reader);

            Radius = reader.ReadUInt16();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(Handle);
            writer.Write(Caster);
            writer.Write(SkillID);

            Position.Write(writer);

            writer.Write(Radius);
        }

        public override NetCommand GetCommand() => NetCommand.NC_BRIEFINFO_MAGICFIELDSPREAD_CMD;

        public override string ToString() => $"{nameof(Handle)}: {Handle}, {nameof(Caster)}: {Caster}, {nameof(SkillID)}: {SkillID}, {nameof(Position)}: {Position}, {nameof(Radius)}: {Radius}";
    }
}
