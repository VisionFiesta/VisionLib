using Vision.Core;
using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;
using Vision.Game.Structs.Common;

namespace Vision.Game.Structs.BriefInfo
{
    public class NcBriefInfoEffectBlastCmd : NetPacketStruct
    {
        public const int Size = 2 + NameN.Name8Len + ShineXYR.Size + 4; // + unk

        public ushort Handle;
        public string EffectName;
        public ShineXYR Coordinates = new ShineXYR();
        public ushort Detach;
        public ushort Scale;
        // $0A1B13CF6BC9B89EA378BE620EA3CE70 flag;

        public override int GetSize() => Size;

        public override void Read(ReaderStream reader)
        {
            Handle = reader.ReadUInt16();
            EffectName = reader.ReadString(NameN.Name8Len);

            Coordinates.Read(reader);

            Detach = reader.ReadUInt16();
            Scale = reader.ReadUInt16();
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(Handle);
            writer.Write(EffectName, NameN.Name8Len);

            Coordinates.Write(writer);

            writer.Write(Detach);
            writer.Write(Scale);
        }

        public override NetCommand GetCommand() => NetCommand.NC_BRIEFINFO_EFFECTBLAST_CMD;

        public override string ToString() => $"{nameof(Handle)}: {Handle}, {nameof(EffectName)}: {EffectName}, {nameof(Coordinates)}: {Coordinates}, {nameof(Detach)}: {Detach}, {nameof(Scale)}: {Scale}";
    }
}
