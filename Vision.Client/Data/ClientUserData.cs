namespace Vision.Client.Data
{
    public class ClientUserData
    {
        public string Username { get; protected set; } = "";
        public string Password { get; protected set; } = "";

        public string LoginServerIP { get; protected set; } = "35.231.44.7"; // NA Server
        public ushort LoginServerPort { get; protected set; } = 9010;

        public string DesiredWorld { get; protected set; } = "";
        public string CharacterName { get; protected set; } = "";

        public ClientUserData() { }

        public ClientUserData(string username, string password, string loginIp, ushort loginPort)
        {
            Username = username;
            Password = password;
            LoginServerIP = loginIp;
            LoginServerPort = loginPort;
        }
    }
}
