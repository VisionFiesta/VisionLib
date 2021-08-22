using System.Diagnostics.CodeAnalysis;
using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.BriefInfo
{
    public class NcBriefInfoEffectCmd : NetPacketStruct
    {
        public byte EffectNum;
        public NcBriefInfoEffectBlastCmd[] Effects;

        public override int GetSize() => 1 + NcBriefInfoEffectBlastCmd.Size;

        public override void Read(ReaderStream reader)
        {
            EffectNum = reader.ReadByte();

            Effects = new NcBriefInfoEffectBlastCmd[EffectNum];

            for (var i = 0; i < EffectNum; i++)
            {
                Effects[i] = new NcBriefInfoEffectBlastCmd();
                Effects[i].Read(reader);
            }
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(EffectNum);

            for (var i = 0; i < EffectNum; i++)
            {
                Effects[i].Write(writer);
            }
        }

        protected override NetCommand GetCommand() => NetCommand.NC_BRIEFINFO_EFFECT_CMD;

        [SuppressMessage("ReSharper", "CoVariantArrayConversion")]
        public override string ToString()
        {
            var effectsString = string.Join(", ", (object[])Effects);
            return $"{nameof(EffectNum)}: {EffectNum}, {nameof(Effects)}: {effectsString}";
        }
    }
}
