using System;
using System.Collections.Generic;
using System.Net;
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
        private readonly EngineLog _engineLog = new EngineLog(typeof(FiestaClient));
        private readonly ClientLog _clientLog = new ClientLog(typeof(FiestaClient));

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

        private readonly SHNManager _shnManager;
        private readonly SHNHashManager _shnHashManager;

        public readonly string SHNHash;

        public ClientSessionData ClientSessionData = new ClientSessionData();

        public FiestaClient(ClientUserData userData)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            UserData = userData;
            StaticClientData = new StaticClientData();

            _shnManager = new SHNManager(StaticClientData.ShinePath, new SHNCrypto());
            _shnHashManager = new SHNHashManager(_shnManager, UserData.Region == GameRegion.GR_NA);

            LoadSHN(StaticClientData.ShinePath);
            _shnHashManager.LoadRemainingHashes();
            if (_shnHashManager.GetFullHash(out var shnHash))
            {
                _engineLog.Debug("Generated SHN hash for client.");
                SHNHash = shnHash;
            } 
            else
            {
                _engineLog.Warning("Failed to generate SHN hash!");
            }

            NetPacketHandlerLoader<NetClientConnection>.LoadHandlers();

            // Network clients
            LoginClient = new NetClientConnection(this, NetConnectionDestination.NCD_LOGIN);
            WorldClient = new NetClientConnection(this, NetConnectionDestination.NCD_WORLDMANAGER);
            ZoneClient = new NetClientConnection(this, NetConnectionDestination.NCD_ZONE);

            // Network services
            LoginService = new LoginService(this);
            WorldService = new WorldService(this);
            ZoneService = new ZoneService(this);

            // Interaction services
            ChatService = new ChatService(this);
            MapService = new MapService(this);

            _clientLog.Info("Initialized Client Services");

            LoginClient.AddDisconnectCallback((dest, endPoint) =>
                LoginService.UpdateState(LoginServiceTrigger.LST_DISCONNECT));
            WorldClient.AddDisconnectCallback((dest, endPoint) =>
                WorldService.UpdateState(WorldServiceTrigger.WST_DISCONNECT));
            ZoneClient.AddDisconnectCallback((dest, endPoint) =>
                _ = ZoneService.UpdateState(ZoneServiceTrigger.ZST_DISCONNECT));
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _engineLog.Error($"Unhandled Exception in context {sender?.GetType().Name}", (Exception) e.ExceptionObject);
        }

        private void LoadSHN(string shnPath)
        {
            var shnManager = new SHNManager(shnPath, new SHNCrypto());

            #region AbnormalState

            var abstateDataProvider = new AbnormalStateDataProvider(_shnManager);
            abstateDataProvider.Initialize();

            #endregion

            #region FaceInfo

            var faceInfoLoader = shnManager.GetSHNLoader(SHNType.FaceInfo);
            faceInfoLoader.Load((shnResult, index) =>
            {
                var face = new CharacterFace(shnResult, index);
                CharacterFace.AllFacesByID.Add(face.ID, face);
            });
            _shnHashManager.AddHash(SHNType.FaceInfo, faceInfoLoader.MD5Hash);

            #endregion

            #region MobInfo

            var mobInfoLoader = shnManager.GetSHNLoader(SHNType.MobInfo);
            mobInfoLoader.Load((shnResult, index) =>
             {
                 var mi = new MobInfo(shnResult, index);
                 MobInfo.AllMobInfosByID.Add(mi.ID, mi);
             });
            _shnHashManager.AddHash(SHNType.MobInfo, mobInfoLoader.MD5Hash);

            #endregion
        }
    }

    public class ClientSessionData
    {
        // Post-Login Global
        public readonly List<NcUserWorldStatusAck.WorldStatusStruct> Worlds = new List<NcUserWorldStatusAck.WorldStatusStruct>();
        public IPEndPoint SelectedWorldEndPoint;

        public ClientGameTime GameTime = new ClientGameTime();
        public Account ClientAccount = new Account();
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


