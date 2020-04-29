using System;
using System.Collections.Generic;
using VisionLib.Client.Data;
using VisionLib.Client.Enums;
using VisionLib.Client.Services;
using VisionLib.Common.Collections;
using VisionLib.Common.Game;
using VisionLib.Common.Game.Content.GameObjects;
using VisionLib.Common.Logging;
using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Packet;
using VisionLib.Core.Struct.User;

namespace VisionLib.Client
{
    public class FiestaClient
    {
        public readonly ClientUserData UserData;
        public readonly ClientData ClientData;

        public readonly FiestaNetConnection LoginClient;
        public readonly FiestaNetConnection WorldClient;
        public readonly FiestaNetConnection ZoneClient;

        public readonly ClientLoginService LoginService;
        public readonly ClientWorldService WorldService;
        public readonly ClientZoneService ZoneService;

        public GameData GameData = new GameData();

        public FiestaClient(ClientUserData userData, ClientData clientData)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            UserData = userData;
            ClientData = clientData;
            
            FiestaNetPacketHandlerLoader.LoadHandlers();

            LoginClient = new FiestaNetClientConnection(this, FiestaNetConnDest.FNCDEST_LOGIN);
            WorldClient = new FiestaNetClientConnection(this, FiestaNetConnDest.FNCDEST_WORLDMANAGER);
            ZoneClient = new FiestaNetClientConnection(this, FiestaNetConnDest.FNCDEST_ZONE);

            LoginService = new ClientLoginService(this);
            WorldService = new ClientWorldService(this);
            ZoneService = new ClientZoneService(this);

            LoginClient.AddDisconnectCallback((dest, endPoint) => LoginService.SetStatus(ClientLoginServiceStatus.CLSS_NOTCONNECTED));
            WorldClient.AddDisconnectCallback((dest, endPoint) => WorldService.SetStatus(ClientWorldServiceStatus.CWSS_NOTCONNECTED));
            ZoneClient.AddDisconnectCallback((dest, endPoint) => ZoneService.SetStatus(ClientZoneServiceStatus.CZSS_NOTCONNECTED));
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            EngineLog.Error($"Unhandled Exception in context {sender.GetType().Name}", (Exception)e.ExceptionObject);
        }
    }

    public class GameData
    { 
        public ClientGameTime GameTime = new ClientGameTime();

        public readonly List<NcUserWorldStatusAck.WorldStatusStruct> Worlds = new List<NcUserWorldStatusAck.WorldStatusStruct>();

        public readonly FastList<GameObject> NearbyGameObjects = new FastList<GameObject>();

        public Account ClientAccount = new Account();
    }
}


