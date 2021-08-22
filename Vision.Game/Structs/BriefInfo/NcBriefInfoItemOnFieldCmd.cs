using System.Diagnostics.CodeAnalysis;
using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.BriefInfo
{
    public class NcBriefInfoItemOnFieldCmd : NetPacketStruct
    {
        public byte ItemNum;
        public NcBriefInfoDropedItemCmd[] Items;

        public override int GetSize() => 1 + (ItemNum * NcBriefInfoDropedItemCmd.Size);

        public override void Read(ReaderStream reader)
        {
            ItemNum = reader.ReadByte();

            Items = new NcBriefInfoDropedItemCmd[ItemNum];

            for (var i = 0; i < ItemNum; i++)
            {
                Items[i] = new NcBriefInfoDropedItemCmd();
                Items[i].Read(reader);
            }
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(ItemNum);

            for (var i = 0; i < ItemNum; i++)
            {
                Items[i].Write(writer);
            }
        }

        protected override NetCommand GetCommand() => NetCommand.NC_BRIEFINFO_ITEMONFIELD_CMD;

        [SuppressMessage("ReSharper", "CoVariantArrayConversion")]
        public override string ToString()
        {
            var itemsString = string.Join(", ", (object[])Items);
            return $"{nameof(ItemNum)}: {ItemNum}, {nameof(Items)}: {itemsString}";
        }

        protected override bool HasMaximumSize() => false;
    }
}
