﻿using System.Diagnostics.CodeAnalysis;
using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.BriefInfo
{
    public class NcBriefInfoMagicFieldInfoCmd : NetPacketStruct
    {
        public byte MagicFieldNum;
        public NcBriefInfoMagicFieldSpreadCmd[] MagicFields;

        public override int GetSize() => 1 + MagicFieldNum * NcBriefInfoMagicFieldSpreadCmd.Size;

        public override void Read(ReaderStream reader)
        {
            MagicFieldNum = reader.ReadByte();

            MagicFields = new NcBriefInfoMagicFieldSpreadCmd[MagicFieldNum];

            for (var i = 0; i < MagicFieldNum; i++)
            {
                MagicFields[i] = new NcBriefInfoMagicFieldSpreadCmd();
                MagicFields[i].Read(reader);
            }
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(MagicFieldNum);

            for (var i = 0; i < MagicFieldNum; i++)
            {
                MagicFields[i].Write(writer);
            }
        }

        protected override NetCommand GetCommand() => NetCommand.NC_BRIEFINFO_MAGICFIELDINFO_CMD;

        protected override bool HasMaximumSize() => false;

        [SuppressMessage("ReSharper", "CoVariantArrayConversion")]
        public override string ToString()
        {
            var fieldsString = string.Join(", ", (object[])MagicFields);
            return $"{nameof(MagicFieldNum)}: {MagicFieldNum}, {nameof(MagicFields)}: {fieldsString}";
        }
    }
}
