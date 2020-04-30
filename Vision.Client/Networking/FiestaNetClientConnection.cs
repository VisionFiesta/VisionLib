using Vision.Core.Networking;
using Vision.Core.Networking.Crypto;
using Vision.Game;

namespace Vision.Client.Networking
{
    public class FiestaNetClientConnection : FiestaNetConnection
    {
        public readonly FiestaClient GameClient;
        public Account Account => GameClient.GameData.ClientAccount;

        public FiestaNetClientConnection(FiestaClient client, FiestaNetConnDest dest) : base(dest, FiestaNetConnDest.FNCDEST_CLIENT)
        {
            GameClient = client;
        }

        public FiestaNetClientConnection(FiestaClient client, FiestaNetConnDest dest, IFiestaNetCrypto crypto) : base(
            dest, FiestaNetConnDest.FNCDEST_CLIENT, crypto)
        {
            GameClient = client;
        }
    }
}
