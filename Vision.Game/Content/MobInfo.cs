using Vision.Core.IO.SHN;

namespace Vision.Game.Content
{
    public class MobInfo
    {
        public ushort ID { get; }
        public string Name { get; }
        public byte Level { get; }
        // public StatsHolder Stats { get; }
        public bool IsNPC { get; }
        public uint Size { get; }

        public MobInfo(SHNResult shnResult, int i)
        {
            ID = shnResult.Read<ushort>(i, "ID");
            Name = shnResult.Read<string>(i, "InxName");
            Level = shnResult.Read<byte>(i, "Level");
            IsNPC = shnResult.Read<bool>(i, "IsNPC");
            Size = shnResult.Read<uint>(i, "Size");
        }
    }
}
