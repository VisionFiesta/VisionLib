using Vision.Core;

namespace Vision.Client.Data
{
    public class ClientUserData
    {
        public string Username { get; protected set; } = "";
        public string Password { get; protected set; } = "";

        public string LoginServerIP { get; protected set; } = "35.231.44.7"; // NA Server
        public ushort LoginServerPort { get; protected set; } = 9010;

        public GameRegion Region { get; protected set; } = GameRegion.GR_NA;

        public string DesiredWorld { get; protected set; } = "";
        public string CharacterName { get; protected set; } = "";

        public bool IsBot { get; protected set; } = true;

        public ClientUserData() { }

        public ClientUserData(string username, string password, string loginServerIP, ushort loginServerPort, GameRegion region)
        {
            Username = username;
            Password = password;
            LoginServerIP = loginServerIP;
            LoginServerPort = loginServerPort;
            Region = region;
        }
    }
}
