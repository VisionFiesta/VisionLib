using VisionLib.Client;
using VisionLib.Common.Networking.Crypto;

namespace VisionLib.Common.Networking
{
    public class FiestaNetClientConnection : FiestaNetConnection
    {
        public readonly FiestaClient GameClient;

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
