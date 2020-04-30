using System;
using System.Linq;
using System.Net;
using System.Threading;
using Vision.Client.Data;
using Vision.Client.Enums;
using Vision.Core.Logging.Loggers;
using Vision.Core.Networking;
using Vision.Game.Structs.User;

namespace Vision.Client.Services
{
    public class ClientLoginService
    {
        public ClientLoginServiceStatus LoginStatus { get; private set; } = ClientLoginServiceStatus.CLSS_NOTCONNECTED;

        // private bool waitForUserInputToConnect = true;

        private readonly FiestaClient _client;
        private FiestaNetConnection LoginConnection => _client.LoginClient;
        private ClientUserData UserData => _client.UserData;
        private ClientData ClientData => _client.ClientData;
        private ClientWorldService WorldService => _client.WorldService;

        private readonly ClientLoginServiceData _data = new ClientLoginServiceData();

        public ClientLoginService(FiestaClient client)
        {
            _client = client;
        }

        public void Update(long timer)
        {
            if (LoginStatus == ClientLoginServiceStatus.CLSS_IDLE)
            {
                // every X seconds refresh worlds
            }
            else
            {
                // BUSY
            }
        }

        public void SetStatus(ClientLoginServiceStatus status)
        {
            LoginStatus = status;
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            switch (LoginStatus)
            {
                case ClientLoginServiceStatus.CLSS_NOTCONNECTED:
                    {
                        ClientLog.Info("ClientLoginService: Disconnected");
                        break;
                    }
                case ClientLoginServiceStatus.CLSS_TRYCONNECT:
                    {
                        ClientLog.Info("ClientLoginService: Connecting...");
                        LoginConnection.Connect(UserData.LoginServerIP, UserData.LoginServerPort);
                        break;
                    }
                case ClientLoginServiceStatus.CLSS_CONNECTED:
                    {
                        ClientLog.Info("ClientLoginService: Connected");
                        new NcUserClientVersionCheckReq(ClientData.VersionKey).Send(LoginConnection);
                        break;
                    }
                case ClientLoginServiceStatus.CLSS_VERIFIED:
                    {
                        ClientLog.Info("ClientLoginService: Version verified");
                        // Thread.Sleep(50);
                        new NcUserUSLoginReq(UserData.Username, UserData.Password).Send(LoginConnection);
                        // new NcUserXTrapReq((byte)ClientData.XTrapVersionHash.Length, ClientData.XTrapVersionHash).Send(LoginConnection);
                        // new STRUCT_NC_USER_XTRAP_REQ((byte) _config.XTrapVersionHash.Length,
                        // _config.XTrapVersionHash).ToPacket().Send(_loginConnection);
                        break;
                    }
                case ClientLoginServiceStatus.CLSS_LOGGEDIN:
                    {
                        ClientLog.Info("ClientLoginService: Logged in");
                        Thread.Sleep(50);
                        new NcUserWorldStatusReq().Send(LoginConnection);
                        break;
                    }
                case ClientLoginServiceStatus.CLSS_GOTWORLDS:
                    {
                        ClientLog.Info("ClientLoginService: Got World list");
                        Thread.Sleep(50);
                        var desiredWorld = _client.GameData.Worlds.First(w => w.WorldName.Equals(UserData.DesiredWorld));
                        var desiredWorldID = desiredWorld?.WorldID ?? 0;

                        new NcUserWorldSelectReq(desiredWorldID).Send(LoginConnection);
                        break;
                    }
                case ClientLoginServiceStatus.CLSS_JOININGWORLD:
                    {
                        ClientLog.Info("ClientLoginService: Starting world connection...");
                        WorldService.SetStatus(ClientWorldServiceStatus.CWSS_TRYCONNECT);
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
                throw new Exception("Got malformed IP for World Server!");
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
