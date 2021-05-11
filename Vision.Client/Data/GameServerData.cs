using Vision.Core;

namespace Vision.Client.Data
{
    public record GameServerData(string LoginServerIP, ushort LoginServerPort, GameRegion Region, string FriendlyName = "Unknown")
    {
        public string LoginServerIP { get; init; } = LoginServerIP;
        public ushort LoginServerPort { get; init; } = LoginServerPort;
        public GameRegion Region { get; init; } = Region;
        public string FriendlyName { get; init; } = FriendlyName;
    }
}
