using Vision.Core.Networking;
using Vision.Core.Streams;
using Vision.Core.Structs;

namespace Vision.Game.Structs.BriefInfo
{
    public class NcBriefInfoChangeWeaponCmd : NetPacketStruct
    {

        public NcBriefInfoChangeUpgradeCmd UpgradeInfo = new();
        public ushort CurrentMobID;
        public byte CurrentKillLevel;

        public override int GetSize() => NcBriefInfoChangeUpgradeCmd.Size + 2 + 1;

        public override void Read(ReaderStream reader)
        {
            UpgradeInfo.Read(reader);

            CurrentMobID = reader.ReadUInt16();
            CurrentKillLevel = reader.ReadByte();
        }

        public override void Write(WriterStream writer)
        {
            UpgradeInfo.Write(writer);
            writer.Write(CurrentMobID);
            writer.Write(CurrentKillLevel);
        }

        protected override NetCommand GetCommand() => NetCommand.NC_BRIEFINFO_CHANGEWEAPON_CMD;

        public override string ToString()
        {
            return $"{nameof(UpgradeInfo)}: [{UpgradeInfo}], {nameof(CurrentMobID)}: {CurrentMobID}, {nameof(CurrentKillLevel)}: {CurrentKillLevel}";
        }
    }
}
