using Vision.Core.Networking;
using Vision.Core.Networking.Crypto;
using Vision.Game;

namespace Vision.Client.Networking
{
    public class NetClientConnection : NetConnectionBase<NetClientConnection>
    {
        public readonly FiestaClient GameClient;
        public Account Account => GameClient.GameData.ClientAccount;

        public NetClientConnection(FiestaClient client, NetConnectionDestination dest) : base(dest, NetConnectionDestination.NCD_CLIENT)
        {
            GameClient = client;
        }

        public NetClientConnection(FiestaClient client, NetConnectionDestination dest, INetCrypto crypto) : base(
            dest, NetConnectionDestination.NCD_CLIENT, crypto)
        {
            GameClient = client;
        }
    }
}
