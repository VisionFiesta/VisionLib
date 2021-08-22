using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Vision.Client.Data;
using Vision.Client.Networking;
using Vision.Client.Services;
using Vision.Core;
using Vision.Core.IO.SHN;
using Vision.Core.Logging.Loggers;
using Vision.Core.Networking;
using Vision.Core.Networking.Packet;
using Vision.Game;
using Vision.Game.Characters.Shape;
using Vision.Game.Content;
using Vision.Game.Content.Data.AbnormalState;
using Vision.Game.Structs.Common;
using Vision.Game.Structs.User;

namespace Vision.Client
{
    public class FiestaClient
    {
        private readonly EngineLog _engineLog = new(typeof(FiestaClient));
        private readonly ClientLog _clientLog = new(typeof(FiestaClient));

        public readonly StaticClientData StaticClientData;
        public readonly ClientUserData UserData;

        public readonly NetClientConnection LoginClient;
        public readonly NetClientConnection WorldClient;
        public readonly NetClientConnection ZoneClient;

        public readonly LoginService LoginService;
        public readonly WorldService WorldService;
        public readonly ZoneService ZoneService;

        public readonly ChatService ChatService;
        public readonly MapService MapService;
        public readonly MovementService MovementService;

        private readonly SHNManager _shnManager;
        private readonly SHNHashManager _shnHashManager;

        public readonly string ShnHash;

        public readonly ClientSessionData ClientSessionData = new();

        public bool Ready { get; }
        
        public FiestaClient(ClientUserData userData, StaticClientData clientData = null)
        {
            var ctorWatch = Stopwatch.StartNew();
            _clientLog.Info("Initializing Client...");
            
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

            UserData = userData;
            StaticClientData = clientData ?? new StaticClientData();

            _shnManager = new SHNManager(StaticClientData.ShinePath, new SHNCrypto());
            _shnHashManager = new SHNHashManager(_shnManager, UserData.Region == GameRegion.GR_NA);

            LoadShn();
            
            _clientLog.Info("SHNs loaded");

            _engineLog.Debug("Loaded SHN hash for client. (WARNING: Using static hash!)");
            ShnHash = StaticClientData.ShnHash;
            // _shnHashManager.LoadRemainingHashes();
            // if (_shnHashManager.GetFullHash(out SHNHash))
            // {
            //     _engineLog.Debug("Generated SHN hash for client.");
            // }
            // else
            // {
            //     _engineLog.Warning("Failed to generate SHN hash!");
            // }

            NetPacketHandlerLoader<NetClientConnection>.LoadHandlers();

            // Network clients
            LoginClient = new NetClientConnection(this, NetConnectionDestination.NCD_LOGIN);
            WorldClient = new NetClientConnection(this, NetConnectionDestination.NCD_WORLDMANAGER);
            ZoneClient = new NetClientConnection(this, NetConnectionDestination.NCD_ZONE);

            var serviceWatch = Stopwatch.StartNew();
            _clientLog.Info("Initializing Client Services...");

            // Network services
            LoginService = new LoginService(this);
            WorldService = new WorldService(this);
            ZoneService = new ZoneService(this);

            // Interaction services
            ChatService = new ChatService(this);
            MapService = new MapService(this);
            MovementService = new MovementService(this);

            serviceWatch.Stop();
            _clientLog.Info($"Initialized Client Services in {serviceWatch.Elapsed.TotalMilliseconds:0.####}ms");

            LoginClient.AddDisconnectCallback((_, _) =>
                LoginService.UpdateState(LoginServiceTrigger.LST_DISCONNECT));
            WorldClient.AddDisconnectCallback((_, _) =>
                WorldService.UpdateState(WorldServiceTrigger.WST_DISCONNECT));
            ZoneClient.AddDisconnectCallback((_, _) =>
                ZoneService.UpdateState(ZoneServiceTrigger.ZST_DISCONNECT));
            
            // _clientLog.Info("Initialized Client NetConn Callbacks");
            
            ctorWatch.Stop();
            _clientLog.Info($"Initialized Client in {ctorWatch.Elapsed.TotalMilliseconds:0.####}ms");
            Ready = true;
        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            _engineLog.Info("Got ProcessExit, logging out!");
            Logout();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _engineLog.Error($"Unhandled Exception in context {sender?.GetType().Name}", (Exception) e.ExceptionObject);
        }

        private void LoadShn()
        {

            #region AbnormalState

            var abstateDataProvider = new AbnormalStateDataProvider(_shnManager, _shnHashManager);
            abstateDataProvider.Initialize();

            #endregion

            #region FaceInfo

            var faceInfoLoader = _shnManager.GetSHNLoader(SHNType.FaceInfo);
            faceInfoLoader.Load((shnResult, index) =>
            {
                var face = new CharacterFace(shnResult, index);
                CharacterFace.AllFacesByID.Add(face.ID, face);
            });
            _shnHashManager.AddHash(SHNType.FaceInfo, faceInfoLoader.MD5Hash);

            #endregion

            #region MobInfo

            var mobInfoLoader = _shnManager.GetSHNLoader(SHNType.MobInfo);
            mobInfoLoader.Load((shnResult, index) =>
             {
                 var mi = new MobInfo(shnResult, index);
                 MobInfo.AllMobInfosByID.Add(mi.ID, mi);
             });
            _shnHashManager.AddHash(SHNType.MobInfo, mobInfoLoader.MD5Hash);

            #endregion
        }

        public void Login()
        {
            LoginService.UpdateState(LoginServiceTrigger.LST_TRY_CONNECT);
        }
        
        public void Logout()
        {
            var logoutPacket = new NetPacket(NetCommand.NC_USER_NORMALLOGOUT_CMD);
            logoutPacket.Send(WorldClient);
            logoutPacket.Send(ZoneClient);
        }

        private void OnExit()
        {
            _clientLog.Info("Logging out before shutdown!");
            Logout();
        }

        public void BusyLoop()
        {
            Thread.Sleep(1);
        }
    }

    public class ClientSessionData
    {
        // Post-Login Global
        public readonly List<NcUserWorldStatusAck.WorldStatusStruct> Worlds = new();
        public IPEndPoint SelectedWorldEndPoint;

        public readonly ClientGameTime GameTime = new();
        public Account ClientAccount = new();
        public byte[] WorldAuthBytes;

        public IPEndPoint ActiveZoneEndPoint;

        // CHAR_OPTION
        public ShortCutData[] ShortCutDatas;
        public KeyMapData[] KeyMapDatas;
        public GameOptionData[] GameOptionDatas;

        public void CleanupLoginData()
        {
            Worlds.Clear();
            SelectedWorldEndPoint = null;
        }

        public void CleanupWorldData()
        {
            ClientAccount = new Account();
            WorldAuthBytes = null;

            ShortCutDatas = null;
            KeyMapDatas = null;
            GameOptionDatas = null;
        }

        public void CleanupZoneData()
        {
            ActiveZoneEndPoint = null;
        }
    }
}


