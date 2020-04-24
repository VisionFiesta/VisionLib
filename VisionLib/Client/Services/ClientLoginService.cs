using System;
using System.Linq;
using System.Net;
using VisionLib.Client.Data;
using VisionLib.Client.Enums;
using VisionLib.Common.Logging;
using VisionLib.Common.Networking;
using VisionLib.Common.Networking.Protocols.User;
using VisionLib.Common.Networking.Structs.User;

namespace VisionLib.Client.Services
{
    public class ClientLoginService
    {
        public ClientLoginServiceStatus LoginStatus { get; private set; } = ClientLoginServiceStatus.CLSS_NOTCONNECTED;

        private readonly FiestaClient _client;
        private FiestaNetConnection LoginConnection => _client.LoginClient;
        private ClientData Config => _client.Config;
        private ClientWorldService WorldService => _client.WorldService;

        private readonly ClientLoginServiceData _data = new ClientLoginServiceData();

        public ClientLoginService(FiestaClient client)
        {
            _client = client;
        }

        private void Update()
        {
            switch (LoginStatus)
            {
                case ClientLoginServiceStatus.CLSS_NOTCONNECTED:
                {
                    Log.Write(LogType.GameLog, LogLevel.Info, "ClientLoginService: Disconnected");
                    break;
                }
                case ClientLoginServiceStatus.CLSS_TRYCONNECT:
                {
                    Log.Write(LogType.GameLog, LogLevel.Info, "ClientLoginService: Connecting...");
                    LoginConnection.Connect(Config.LoginServerIP, Config.LoginServerPort);
                    break;
                }
                case ClientLoginServiceStatus.CLSS_CONNECTED:
                {
                    Log.Write(LogType.GameLog, LogLevel.Info, "ClientLoginService: Connected");
                    new STRUCT_NC_USER_CLIENT_VERSION_CHECK_REQ(Config.BinMD5, Config.ClientVersionData).ToPacket().Send(LoginConnection);
                    break;
                }
                case ClientLoginServiceStatus.CLSS_VERIFIED:
                {
                    Log.Write(LogType.GameLog, LogLevel.Info, "ClientLoginService: Version verified");
                    new STRUCT_NC_USER_US_LOGIN_REQ(Config.Username, Config.Password)
                        .ToPacket().Send(LoginConnection);
                    // new STRUCT_NC_USER_XTRAP_REQ((byte) _config.XTrapVersionHash.Length,
                        // _config.XTrapVersionHash).ToPacket().Send(_loginConnection);
                    break;
                }
                case ClientLoginServiceStatus.CLSS_LOGGEDIN:
                {
                    Log.Write(LogType.GameLog, LogLevel.Info, "ClientLoginService: Logged in");
                    new PROTO_NC_USER_WORLD_STATUS_REQ().Send(LoginConnection);
                    break;
                }
                case ClientLoginServiceStatus.CLSS_GOTWORLDS:
                {
                    Log.Write(LogType.GameLog, LogLevel.Info, "ClientLoginService: Got World list");

                    var isya = _client.GameData.Worlds.First(w => w.WorldName.Equals("ISYA"));

                    new STRUCT_NC_USER_WORLDSELECT_REQ(isya.WorldID).ToPacket().Send(LoginConnection);
                    break;
                }
                case ClientLoginServiceStatus.CLSS_JOININGWORLD:
                {
                    Log.Write(LogType.GameLog, LogLevel.Info, "ClientLoginService: Starting world connection...");
                    WorldService.SetStatus(ClientWorldServiceStatus.CWSS_TRYCONNECT);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SetStatus(ClientLoginServiceStatus status)
        {
            LoginStatus = status;
            Update();
        }

        public void SetWMTransferKey(byte[] transferKey) => _data.WmTransferKey = transferKey;

        public byte[] GetWMTransferKey() => _data.WmTransferKey;

        public void SetWMEndpoint(string worldIP, ushort worldPort)
        {
            if (IPAddress.TryParse(worldIP, out var trueWorldIP))
            {
                _data.WorldEndPoint = new IPEndPoint(trueWorldIP, worldPort);
            }
            else
            {
                throw new Exception("Got bad IP for World Server!");
            }
        }

        public IPEndPoint GetWMEndPoint() => _data.WorldEndPoint;
    }

    public class ClientLoginServiceData
    {
        public byte[] WmTransferKey;
        public IPEndPoint WorldEndPoint;
    }
}
