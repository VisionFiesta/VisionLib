using Vision.Client.Data;
using Vision.Client.Networking;
using Vision.Core.Logging.Loggers;
using Vision.Game.Characters;

namespace Vision.Client.Services
{
    public abstract class ClientServiceBase
    {
        protected readonly ClientLog ClientLogger;
        protected readonly EngineLog EngineLogger;

        protected readonly FiestaClient Client;

        protected ClientUserData UserData => Client.UserData;
        protected StaticClientData StaticClientData => Client.StaticClientData;
        protected ClientSessionData ClientSessionData => Client.ClientSessionData;
        protected WorldCharacter ActiveAvatar => ClientSessionData.ClientAccount.ActiveAvatar;
        protected Character ActiveCharacter => ClientSessionData.ClientAccount.ActiveCharacter;

        protected NetClientConnection LoginConnection => Client.LoginClient;
        protected LoginService LoginService => Client.LoginService;
        protected NetClientConnection WorldConnection => Client.WorldClient;
        protected WorldService WorldService => Client.WorldService;
        protected NetClientConnection ZoneConnection => Client.ZoneClient;
        protected ZoneService ZoneService => Client.ZoneService;

        protected ChatService ChatService => Client.ChatService;
        protected MapService MapService => Client.MapService;
        protected MovementService MovementService => Client.MovementService;

        protected ClientServiceBase(FiestaClient client)
        {
            Client = client;
            ClientLogger = new ClientLog(GetType());
            EngineLogger = new EngineLog(GetType());
        }
    }
}
