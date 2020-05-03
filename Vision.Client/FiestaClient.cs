using System;
using System.Collections.Generic;

using Vision.Client.Configuration;
using Vision.Client.Data;
using Vision.Client.Enums;
using Vision.Client.Networking;
using Vision.Client.Services;
using Vision.Core.Collections;
using Vision.Core.IO.SHN;
using Vision.Core.Logging.Loggers;
using Vision.Core.Networking;
using Vision.Core.Networking.Packet;
using Vision.Game;
using Vision.Game.Characters.Shape;
using Vision.Game.Content;
using Vision.Game.Content.Data.AbnormalState;
using Vision.Game.Enums;
using Vision.Game.Structs.User;

namespace Vision.Client
{
    public class FiestaClient
    {
        public readonly ClientUserData UserData;
        public readonly ClientData ClientData;

        public readonly NetClientConnection LoginClient;
        public readonly NetClientConnection WorldClient;
        public readonly NetClientConnection ZoneClient;

        public readonly ClientLoginService LoginService;
        public readonly ClientWorldService WorldService;
        public readonly ClientZoneService ZoneService;

        public readonly ClientChatService ChatService;

        public GameData GameData = new GameData();

        public FiestaClient(ClientUserData userData, ClientData clientData, GameConfiguration gameConfig)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            UserData = userData;
            ClientData = clientData;

            LoadSHN(gameConfig.ShinePath);
            
            NetPacketHandlerLoader<NetClientConnection>.LoadHandlers();

            LoginClient = new NetClientConnection(this, NetConnectionDestination.NCD_LOGIN);
            WorldClient = new NetClientConnection(this, NetConnectionDestination.NCD_WORLDMANAGER);
            ZoneClient = new NetClientConnection(this, NetConnectionDestination.NCD_ZONE);

            LoginService = new ClientLoginService(this);
            WorldService = new ClientWorldService(this);
            ZoneService = new ClientZoneService(this);

            ChatService = new ClientChatService(this);

            LoginClient.AddDisconnectCallback((dest, endPoint) => LoginService.SetStatus(ClientLoginServiceStatus.CLSS_NOTCONNECTED));
            WorldClient.AddDisconnectCallback((dest, endPoint) => WorldService.SetStatus(ClientWorldServiceStatus.CWSS_NOTCONNECTED));
            ZoneClient.AddDisconnectCallback((dest, endPoint) => ZoneService.SetStatus(ClientZoneServiceStatus.CZSS_NOTCONNECTED));
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            EngineLog.Error($"Unhandled Exception in context {sender?.GetType().Name}", (Exception)e.ExceptionObject);
        }

        private static void LoadSHN(string shnPath)
        {
            SHNManager.Initialize(shnPath, new SHNCrypto());

            #region AbnormalState
            AbnormalStateDataProvider.Initialize();
            #endregion

            #region FaceInfo
            SHNManager.GetSHNLoader(SHNType.FaceInfo).Load((shnResult, index) =>
            {
                var face = new CharacterFace(shnResult, index);
                CharacterFace.AllFacesByID.Add(face.ID, face);
            });
            #endregion

            #region MobInfo
            SHNManager.GetSHNLoader(SHNType.MobInfo).Load((shnResult, index) =>
            {
                var mi = new MobInfo(shnResult, index);
                SHNData.AllMobInfosByID.Add(mi.ID, mi);
            });
            #endregion
        }
    }

    public static class SHNData
    {
        public static readonly Dictionary<ushort, MobInfo> AllMobInfosByID = new FastDictionary<ushort, MobInfo>();
    }

    public class GameData
    {
        public ClientGameTime GameTime = new ClientGameTime();

        public readonly List<NcUserWorldStatusAck.WorldStatusStruct> Worlds = new List<NcUserWorldStatusAck.WorldStatusStruct>();

        public Account ClientAccount = new Account();
    }
}


