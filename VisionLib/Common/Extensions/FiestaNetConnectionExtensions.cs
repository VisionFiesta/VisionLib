using VisionLib.Client;
using VisionLib.Common.Game;
using VisionLib.Common.Networking;

namespace VisionLib.Common.Extensions
{
    public static class FiestaNetConnectionExtensions
    {
        public static FiestaClient GetClient(this FiestaNetConnection connection)
        {
            return connection is FiestaNetClientConnection clientConnection ? clientConnection.GameClient : null;
        }

        public static Account GetAccount(this FiestaNetConnection connection)
        {
            return connection is FiestaNetClientConnection clientConnection
                ? clientConnection.GameClient.GameData.ClientAccount
                : null;
        }
    }
}
