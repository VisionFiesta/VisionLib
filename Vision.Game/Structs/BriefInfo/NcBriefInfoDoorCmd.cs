using System.Diagnostics.CodeAnalysis;
using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.BriefInfo
{
    public class NcBriefInfoDoorCmd : NetPacketStruct
    {
        public byte DoorNum;
        public NcBriefInfoBuildDoorCmd[] Doors;

        public override int GetSize() => 1 + NcBriefInfoBuildDoorCmd.Size;

        public override void Read(ReaderStream reader)
        {
            DoorNum = reader.ReadByte();

            Doors = new NcBriefInfoBuildDoorCmd[DoorNum];

            for (var i = 0; i < DoorNum; i++)
            {
                Doors[i] = new NcBriefInfoBuildDoorCmd();
                Doors[i].Read(reader);
            }
        }

        public override void Write(WriterStream writer)
        {
            writer.Write(DoorNum);

            for (var i = 0; i < DoorNum; i++)
            {
                Doors[i].Write(writer);
            }
        }

        public override NetCommand GetCommand() => NetCommand.NC_BRIEFINFO_DOOR_CMD;

        [SuppressMessage("ReSharper", "CoVariantArrayConversion")]
        public override string ToString()
        {
            var doorsString = string.Join(", ", (object[])Doors);
            return $"{nameof(DoorNum)}: {DoorNum}, {nameof(Doors)}: {doorsString}";
        }
    }
}
