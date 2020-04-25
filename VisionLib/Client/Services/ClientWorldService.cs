using System;

using VisionLib.Client.Data;
using VisionLib.Client.Enums;
using VisionLib.Common.Extensions;
using VisionLib.Common.Logging;
using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Protocols.Misc;
using VisionLib.Core.Struct.Char;
using VisionLib.Core.Struct.User;

namespace VisionLib.Client.Services
{
    public class ClientWorldService
    {
        public ClientWorldServiceStatus WorldStatus { get; private set; } = ClientWorldServiceStatus.CWSS_NOTCONNECTED;

        private readonly FiestaClient _client;
        private FiestaNetConnection WorldConnection => _client.WorldClient;
        private ClientUserData Config => _client.UserData;
        private ClientLoginService LoginService => _client.LoginService;

        private readonly ClientWorldServiceData _data = new ClientWorldServiceData();

        public ClientWorldService(FiestaClient client)
        {
            _client = client;
        }

        public void SetStatus(ClientWorldServiceStatus status)
        {
            WorldStatus = status;
            Update();
        }

        private void Update()
        {
            switch (WorldStatus)
            {
                case ClientWorldServiceStatus.CWSS_NOTCONNECTED:
                    {
                        Log.Write(LogType.GameLog, LogLevel.Info, "ClientWorldService: Disconnected");
                        break;
                    }
                case ClientWorldServiceStatus.CWSS_TRYCONNECT:
                    {
                        Log.Write(LogType.GameLog, LogLevel.Info, "ClientWorldService: Connecting...");
                        WorldConnection.Connect(LoginService.GetWMEndPoint());
                        break;
                    }
                case ClientWorldServiceStatus.CWSS_CONNECTED:
                    {
                        Log.Write(LogType.GameLog, LogLevel.Info, "ClientWorldService: Connected");
                        new NcUserLoginWorldReq(
                                Config.Username,
                                LoginService.GetWMTransferKey())
                            .ToPacket().Send(WorldConnection);
                        break;
                    }
                case ClientWorldServiceStatus.CWSS_GOTAVATARS:
                    {
                        Log.Write(LogType.GameLog, LogLevel.Info, "ClientWorldService: Got Avatars");

                        new PROTO_NC_MISC_GAMETIME_REQ().Send(WorldConnection);
                        break;
                    }
                case ClientWorldServiceStatus.CWSS_GOTGAMETIME:
                    {
                        Log.Write(LogType.GameLog, LogLevel.Info, "ClientWorldService: Got GameTime");

                        var avatar = _client.GameData.ClientAccount.Avatars.First(a => a.Name == Config.CharacterName);
                        _client.GameData.ClientAccount.SelectAvatar(avatar.Slot);

                        new NcCharLoginReq(avatar.Slot).ToPacket().Send(WorldConnection);
                        break;
                    }
                case ClientWorldServiceStatus.CWSS_CHARLOGGEDIN:
                    {
                        Log.Write(LogType.GameLog, LogLevel.Info, "ClientWorldService: Logged in");



                        break;
                    }
                case ClientWorldServiceStatus.CWSS_JOINEDZONE:
                    {
                        Log.Write(LogType.GameLog, LogLevel.Info, "ClientWorldService: Joined Zone");
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SetWMHandle(ushort handle) => _data.WMHandle = handle;

        public ushort GetWMHandle() => _data.WMHandle;
    }

    public class ClientWorldServiceData
    {
        public ushort WMHandle;
    }
}
