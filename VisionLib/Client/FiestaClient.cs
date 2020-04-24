using System.Collections.Generic;
using VisionLib.Client.Data;
using VisionLib.Client.Services;
using VisionLib.Common.Game;
using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Packet;
using VisionLib.Common.Networking.Structs.User;

namespace VisionLib.Client
{
    public class FiestaClient
    {
        public readonly ClientData Config;

        public readonly FiestaNetConnection LoginClient;
        public readonly FiestaNetConnection WorldClient;
        public readonly FiestaNetConnection ZoneClient;

        public readonly ClientLoginService LoginService;
        public readonly ClientWorldService WorldService;

        public GameData GameData = new GameData();

        public FiestaClient(ClientData config)
        {
            Config = config;
            
            FiestaNetPacketHandlerLoader.LoadHandlers();


            LoginClient = new FiestaNetClientConnection(this, FiestaNetConnDest.FNCDEST_LOGIN);
            WorldClient = new FiestaNetClientConnection(this, FiestaNetConnDest.FNCDEST_WORLDMANAGER);
            ZoneClient = new FiestaNetClientConnection(this, FiestaNetConnDest.FNCDEST_ZONE);

            LoginService = new ClientLoginService(this);
            WorldService = new ClientWorldService(this);
        }
    }

    public class GameData
    { 
        public ClientGameTime GameTime;

        public readonly List<STRUCT_NC_USER_WORLD_STATUS_ACK.WORLD_STATUS> Worlds = new List<STRUCT_NC_USER_WORLD_STATUS_ACK.WORLD_STATUS>();

        public Account ClientAccount = new Account();

        // Login<->World
        public ClientLoginServiceData LoginServiceData = new ClientLoginServiceData();
    }
}
