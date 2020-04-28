using System.Net;
using VisionLib.Client.Data;
using VisionLib.Client.Enums;
using VisionLib.Common.Extensions;
using VisionLib.Common.Logging;
using VisionLib.Common.Networking;
using VisionLib.Core.Struct.Char;
using VisionLib.Core.Struct.Misc;
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
        private ClientZoneService ZoneService => _client.ZoneService;

        private readonly ClientWorldServiceData _data = new ClientWorldServiceData();

        public ClientWorldService(FiestaClient client)
        {
            _client = client;
        }

        public void SetStatus(ClientWorldServiceStatus status)
        {
            WorldStatus = status;
            UpdateStatus();
        }

        public void Update(long timer)
        {
            if (WorldStatus == ClientWorldServiceStatus.CWSS_IDLE)
            {

            }
            else
            {
                // BUSY
            }
        }

        private void UpdateStatus()
        {
            switch (WorldStatus)
            {
                case ClientWorldServiceStatus.CWSS_NOTCONNECTED:
                    {
                        ClientLog.Info("ClientWorldService: Disconnected");
                        break;
                    }
                case ClientWorldServiceStatus.CWSS_TRYCONNECT:
                    {
                        ClientLog.Info("ClientWorldService: Connecting...");
                        WorldConnection.Connect(LoginService.GetWMEndPoint());
                        break;
                    }
                case ClientWorldServiceStatus.CWSS_CONNECTED:
                    {
                        ClientLog.Info("ClientWorldService: Connected");
                        new NcUserLoginWorldReq(
                                Config.Username,
                                LoginService.GetWMTransferKey())
                            .Send(WorldConnection);
                        break;
                    }
                case ClientWorldServiceStatus.CWSS_GOTAVATARS:
                    {
                        ClientLog.Info("ClientWorldService: Got Avatars");

                        new NcMiscGameTimeReq().Send(WorldConnection);
                        break;
                    }
                case ClientWorldServiceStatus.CWSS_GOTGAMETIME:
                    {
                        ClientLog.Info("ClientWorldService: Got GameTime");

                        var avatar = _client.GameData.ClientAccount.Avatars.First(a => a.Name == Config.CharacterName);
                        _client.GameData.ClientAccount.ChooseCharacter(avatar.CharNo);

                        new NcCharLoginReq(avatar.Slot).Send(WorldConnection);
                        break;
                    }
                case ClientWorldServiceStatus.CWSS_CHARLOGGEDIN:
                    {
                        ClientLog.Info("ClientWorldService: Character Logged in, joining zone...");
                        ZoneService.SetStatus(ClientZoneServiceStatus.CZSS_TRYCONNECT);
                        break;
                    }
                case ClientWorldServiceStatus.CWSS_JOINEDZONE:
                    {
                        ClientLog.Info("ClientWorldService: Joined Zone");
                        SetStatus(ClientWorldServiceStatus.CWSS_IDLE);
                        break;
                    }
                case ClientWorldServiceStatus.CWSS_IDLE:
                    {
                        ClientLog.Info("ClientWorldService: Idling");
                        break;
                    }
            }
        }

        public void SetInitialZoneEndpoint(IPEndPoint zoneEndPoint) => _data.InitialZoneEndpoint = zoneEndPoint;

        public IPEndPoint GetInitialZoneEndpoint() => _data.InitialZoneEndpoint;
    }

    public class ClientWorldServiceData
    {
        public IPEndPoint InitialZoneEndpoint;
    }
}
