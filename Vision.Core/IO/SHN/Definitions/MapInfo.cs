namespace Vision.Core.IO.SHN.Definitions
{
    [Definition]
    public class MapInfo
    {
        [Identity]
        public ushort ID { get; private set; }
        public string MapName { get; private set; }
        public string Name { get; private set; }
        public byte IsWMLink { get; private set; }
        public uint RegenX { get; private set; }
        public uint RegenY { get; private set; }
        public byte KingdomMap { get; private set; }
        public string MapFolderName { get; private set; }
        public bool InSide { get; private set; }
        public int Sight { get; private set; }
    }
}
