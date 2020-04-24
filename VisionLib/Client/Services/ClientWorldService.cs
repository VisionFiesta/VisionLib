using System;

using VisionLib.Client.Data;
using VisionLib.Client.Enums;
using VisionLib.Common.Extensions;
using VisionLib.Common.Logging;
using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Protocols.Misc;
using VisionLib.Common.Networking.Structs.Char;
using VisionLib.Common.Networking.Structs.User;

namespace VisionLib.Client.Services
{
    public class ClientWorldService
    {
        public ClientWorldStatus WorldStatus { get; private set; } = ClientWorldStatus.CWS_NOTCONNECTED;

        private readonly FiestaClient _client;
        private FiestaNetConnection WorldConnection => _client.WorldClient;
        private ClientData Config => _client.Config;
        private ClientLoginService LoginService => _client.LoginService;

        private ClientWorldServiceData _data = new ClientWorldServiceData();

        public ClientWorldService(FiestaClient client)
        {
            _client = client;
        }

        public void SetStatus(ClientWorldStatus status)
        {
            WorldStatus = status;
            Update();
        }

        private void Update()
        {
            switch (WorldStatus)
            {
                case ClientWorldStatus.CWS_NOTCONNECTED:
                    {
                        Log.Write(LogType.GameLog, LogLevel.Info, "ClientWorldService: Disconnected");
                        break;
                    }
                case ClientWorldStatus.CWS_TRYCONNECT:
                    {
                        Log.Write(LogType.GameLog, LogLevel.Info, "ClientWorldService: Connecting...");
                        WorldConnection.Connect(LoginService.GetWMEndPoint());
                        break;
                    }
                case ClientWorldStatus.CWS_CONNECTED:
                    {
                        Log.Write(LogType.GameLog, LogLevel.Info, "ClientWorldService: Connected");
                        new STRUCT_NC_USER_LOGINWORLD_REQ(
                                Config.Username,
                                LoginService.GetWMTransferKey())
                            .ToPacket().Send(WorldConnection);
                        break;
                    }
                case ClientWorldStatus.CWS_GOTAVATARS:
                    {
                        Log.Write(LogType.GameLog, LogLevel.Info, "ClientWorldService: Got Avatars");

                        new PROTO_NC_MISC_GAMETIME_REQ().Send(WorldConnection);
                        break;
                    }
                case ClientWorldStatus.CWS_GOTGAMETIME:
                    {
                        Log.Write(LogType.GameLog, LogLevel.Info, "ClientWorldService: Got GameTime");

                        var avatar = _client.GameData.ClientAccount.Avatars.First(a => a.Name == Config.CharacterName);
                        _client.GameData.ClientAccount.SelectAvatar(avatar.Slot);

                        new STRUCT_NC_CHAR_LOGIN_REQ(avatar.Slot).ToPacket().Send(WorldConnection);
                        break;
                    }
                case ClientWorldStatus.CWS_CHARLOGGEDIN:
                    {
                        Log.Write(LogType.GameLog, LogLevel.Info, "ClientWorldService: Logged in");



                        break;
                    }
                case ClientWorldStatus.CWS_JOINEDZONE:
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
