using System;
using System.Collections.Generic;

using Vision.Client.Configuration;
using Vision.Client.Data;
using Vision.Client.Enums;
using Vision.Client.Networking;
using Vision.Client.Services;
using Vision.Core.Common.Collections;
using Vision.Core.Common.IO.SHN;
using Vision.Core.Common.Logging.Loggers;
using Vision.Core.Networking;
using Vision.Core.Networking.Packet;
using Vision.Game;
using Vision.Game.Characters.Shape;
using Vision.Game.Content;
using Vision.Game.Structs.User;

namespace Vision.Client
{
    public class FiestaClient
    {
        public readonly ClientUserData UserData;
        public readonly ClientData ClientData;

        public readonly FiestaNetClientConnection LoginClient;
        public readonly FiestaNetClientConnection WorldClient;
        public readonly FiestaNetClientConnection ZoneClient;

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
            
            FiestaNetPacketHandlerLoader<FiestaNetClientConnection>.LoadHandlers();

            LoginClient = new FiestaNetClientConnection(this, FiestaNetConnDest.FNCDEST_LOGIN);
            WorldClient = new FiestaNetClientConnection(this, FiestaNetConnDest.FNCDEST_WORLDMANAGER);
            ZoneClient = new FiestaNetClientConnection(this, FiestaNetConnDest.FNCDEST_ZONE);

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
            EngineLog.Error($"Unhandled Exception in context {sender.GetType().Name}", (Exception)e.ExceptionObject);
        }

        private static void LoadSHN(string shnPath)
        {
            SHNManager.Initialize(shnPath, new SHNCrypto());

            #region FaceInfo
            new SHNLoader<byte, CharacterFace>(SHNType.FaceInfo).Load((shnResult, index) =>
            {
                var face = new CharacterFace(shnResult, index);
                CharacterFace.AllFacesByID.Add(face.ID, face);
            });
            #endregion

            #region MobInfo
            new SHNLoader<ushort, MobInfo>(SHNType.MobInfo).Load((shnResult, index) =>
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


