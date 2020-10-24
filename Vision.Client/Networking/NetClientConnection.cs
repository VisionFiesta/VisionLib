using Vision.Client.Services;
using Vision.Core.Networking;
using Vision.Game;

namespace Vision.Client.Networking
{
    public class NetClientConnection : NetConnectionBase<NetClientConnection>
    {
        public readonly FiestaClient GameClient;
        public Account Account => GameClient.ClientSessionData.ClientAccount;

        public NetClientConnection(FiestaClient client, NetConnectionDestination dest) : base(dest, NetConnectionDestination.NCD_CLIENT)
        {
            GameClient = client;
        }

        public void UpdateLoginService(LoginServiceTrigger trigger) =>
            GameClient.LoginService.UpdateState(trigger);

        public void UpdateWorldService(WorldServiceTrigger trigger) =>
            GameClient.WorldService.UpdateState(trigger);

        public void UpdateZoneService(ZoneServiceTrigger trigger) =>
            GameClient.ZoneService.UpdateState(trigger);

    }
}
