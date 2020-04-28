using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using VisionLib.Client.Data;
using VisionLib.Client.Enums;
using VisionLib.Common.Logging;
using VisionLib.Common.Networking;
using VisionLib.Core.Struct.Map;

namespace VisionLib.Client.Services
{
    public class ClientZoneService
    {
        public ClientZoneServiceStatus ZoneStatus { get; private set; } = ClientZoneServiceStatus.CZSS_NOTCONNECTED;

        private readonly FiestaClient _client;
        private FiestaNetConnection ZoneConnection => _client.ZoneClient;
        private ClientWorldService WorldService => _client.WorldService;

        private readonly ClientZoneServiceData _data = new ClientZoneServiceData();
        private readonly CharClientDataStatus _clientDataStatus = new CharClientDataStatus();

        public ClientZoneService(FiestaClient client)
        {
            _client = client;
        }

        public void SetStatus(ClientZoneServiceStatus status)
        {
            ZoneStatus = status;
            Update();
        }

        private void Update()
        {
            switch (ZoneStatus)
            {
                case ClientZoneServiceStatus.CZSS_NOTCONNECTED:
                    {
                        ClientLog.Info("ClientZoneService: Disconnected");
                        break;
                    }
                case ClientZoneServiceStatus.CZSS_TRYCONNECT:
                    {
                        ClientLog.Info("ClientZoneService: Connecting...");
                        ZoneConnection.Connect(WorldService.GetInitialZoneEndpoint());
                        break;
                    }
                case ClientZoneServiceStatus.CZSS_CONNECTED:
                    {
                        ClientLog.Info("ClientZoneService: Connected");
                        var acct = _client.GameData.ClientAccount;
                        _clientDataStatus.Reset();
                        new NcMapLoginReq(acct.AccountID, acct.ActiveCharacter.Name, _client.ClientData.SHNHash).Send(ZoneConnection);
                        break;
                    }
                case ClientZoneServiceStatus.CZSS_GOTCHARDATA:
                    {
                        ClientLog.Info("ClientZoneService: Got all character data");

                        var act = _client.GameData.ClientAccount;

                        break;
                    }
                case ClientZoneServiceStatus.CZSS_MAPLOGINACK:
                    {
                        WorldService.SetStatus(ClientWorldServiceStatus.CWSS_JOINEDZONE);
                        ClientLog.Info("ClientZoneService: Map Login Complete");
                        break;
                    }
            }
        }

        public void SetZoneEndpoint(IPEndPoint zoneEndPoint)
        {
            _data.CurrentZoneEndPoint = zoneEndPoint;
        }

        public void UpdateCharData(CharClientDataType type)
        {
            ClientLog.Debug($"UpdateCharData: {type}");
            _clientDataStatus.Set(type);

            if (_clientDataStatus.ReceivedAll)
            {
                SetStatus(ClientZoneServiceStatus.CZSS_GOTCHARDATA);
            }
        }
    }

    public enum CharClientDataType : byte
    {
        CCDT_BASE = 0,
        CCDT_SHAPE = 1,
        CCDT_QUESTDOING = 2,
        CCDT_QUESTDONE = 3,
        CCDT_QUESTREAD = 4,
        CCDT_QUESTREPEAT = 5,
        CCDT_SKILL = 6,
        CCDT_PASSIVE = 7,
        CCDT_ITEM1 = 8,
        CCDT_ITEM2 = 9,
        CCDT_ITEM3 = 10,
        CCDT_ITEM4 = 11,
        CCDT_TITLE = 12,
        CCDT_CHARGEDBUFF = 13,
        CCDT_GAME = 14,
        CCDT_COININFO = 15
    }

    public class CharClientDataStatus
    {
        private bool[] _allData = new bool[16];

        public bool ReceivedAll => _allData.All(a => a);

        public void Set(CharClientDataType type) => _allData[(byte) type] = true;

        public bool Get(CharClientDataType type) => _allData[(byte)type];

        public void Reset() => _allData = new bool[16];
    }

    public class ClientZoneServiceData
    {
        public IPEndPoint CurrentZoneEndPoint;
    }
}
