using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Stateless;
using Vision.Game.Structs.Map;

namespace Vision.Client.Services
{
    public class ZoneService : ClientServiceBase
    {
        private readonly StateMachine<ZoneServiceState, ZoneServiceTrigger> _zoneStateMachine =
            new(ZoneServiceState.ZSS_DISCONNECTED, FiringMode.Queued);

        private readonly ZoneCharacterDataManager.CharClientDataStatus _charClientDataStatus = new();

        public ZoneService(FiestaClient client) : base(client)
        {
            var watch = Stopwatch.StartNew();
            ClientLogger.Debug("Initializing...");
            
            #region State machine setup
            _zoneStateMachine.OnTransitioned(transition =>
            {
                if (transition.Source != transition.Destination)
                {
                    ClientLogger.Debug(
                        $"State Change - From {transition.Source} to {transition.Destination} because {transition.Trigger}");
                }
            });

            _zoneStateMachine.OnUnhandledTrigger((state, trigger) =>
            {
                ClientLogger.Warning($"Got unhandled trigger for {state} : {trigger}");
            });

            _zoneStateMachine.Configure(ZoneServiceState.ZSS_DISCONNECTING)
                .OnEntry(OnDisconnectingEntry)
                .Permit(ZoneServiceTrigger.ZST_DISCONNECT, ZoneServiceState.ZSS_DISCONNECTED);

            _zoneStateMachine.Configure(ZoneServiceState.ZSS_DISCONNECTED)
                .OnEntry(OnDisconnectedEntry)
                .Permit(ZoneServiceTrigger.ZST_TRY_CONNECT, ZoneServiceState.ZSS_CONNECTING);

            _zoneStateMachine.Configure(ZoneServiceState.ZSS_CONNECTING)
                .OnEntry(OnConnectingEntry)
                .Permit(ZoneServiceTrigger.ZST_DISCONNECT, ZoneServiceState.ZSS_DISCONNECTED)
                .Permit(ZoneServiceTrigger.ZST_CONNECT_OK, ZoneServiceState.ZSS_CONNECTED)
                .Permit(ZoneServiceTrigger.ZST_CONNECT_FAIL, ZoneServiceState.ZSS_DISCONNECTED);

            _zoneStateMachine.Configure(ZoneServiceState.ZSS_CONNECTED)
                .OnEntry(OnConnectedEntry)
                .Permit(ZoneServiceTrigger.ZST_DISCONNECT, ZoneServiceState.ZSS_DISCONNECTED)
                .Permit(ZoneServiceTrigger.ZST_LOGIN_OK, ZoneServiceState.ZSS_LOGGED_IN)
                .Permit(ZoneServiceTrigger.ZST_LOGIN_FAIL, ZoneServiceState.ZSS_DISCONNECTING);

            _zoneStateMachine.Configure(ZoneServiceState.ZSS_LOGGED_IN)
                .SubstateOf(ZoneServiceState.ZSS_CONNECTED)
                .OnEntryAsync(OnLoggedInEntry)
                .OnExit(OnLoggedInExit)
                .Permit(ZoneServiceTrigger.ZST_MAP_LOAD_OK, ZoneServiceState.ZSS_LOGGED_IN_MAP);

            _zoneStateMachine.Configure(ZoneServiceState.ZSS_LOGGED_IN_MAP)
                .SubstateOf(ZoneServiceState.ZSS_LOGGED_IN)
                .OnEntryAsync(OnLoggedInMapEntry);
            #endregion
            
            watch.Stop();
            ClientLogger.Info($"Initialized in {watch.Elapsed.TotalMilliseconds:0.####}ms");
        }

        public ZoneServiceState GetState => _zoneStateMachine.State;

        public void UpdateState(ZoneServiceTrigger trigger)
        {
            _zoneStateMachine.Deactivate();
            _zoneStateMachine.Fire(trigger);
            _zoneStateMachine.Activate();
        }
        
        public async Task UpdateStateAsync(ZoneServiceTrigger trigger)
        {
            await _zoneStateMachine.DeactivateAsync();
            await _zoneStateMachine.FireAsync(trigger);
            await _zoneStateMachine.ActivateAsync();
        }
        
        private void OnDisconnectingEntry()
        {
            ClientLogger.Info("Disconnecting...");
        }

        private void OnDisconnectedEntry()
        {
            ClientLogger.Info("Disconnected");
        }

        private void OnConnectingEntry()
        {
            ClientLogger.Info("Connecting...");
            var zoneEp = ClientSessionData.ActiveZoneEndPoint;
            ZoneConnection.Connect(zoneEp.Address.ToString(), (ushort)zoneEp.Port);
        }

        private void OnConnectedEntry()
        {
            ClientLogger.Info("Connected");

            WorldService.UpdateState(WorldServiceTrigger.WST_CONNECT_ZONE_OK);

            var acct = ClientSessionData.ClientAccount;
            if (Client.ShnHash != null)
            {
                ClientLogger.Debug("Sending MapLoginReq");
                new NcMapLoginReq(acct.AccountId, acct.ActiveAvatar.CharName, Client.ShnHash).Send(ZoneConnection);
            }
            else
            {
                ClientLogger.Error("Cannot log in - SHN Hash unavailable");
            }
        }

        private async Task OnLoggedInEntry()
        {
            ClientLogger.Info("Logged in");
            await WorldService.UpdateStateAsync(WorldServiceTrigger.WST_LOGIN_ZONE_OK);
            await MapService.LoadMap("RouCos01", 
                (ignored) => { },
                async millis => await UpdateStateAsync(ZoneServiceTrigger.ZST_MAP_LOAD_OK));
        }

        private void OnLoggedInExit()
        {
            ClientLogger.Info("Logged out");
            ClientSessionData.CleanupZoneData();
        }

        private async Task OnLoggedInMapEntry()
        {
            ClientLogger.Info("Map logged in");
            await new NcMapLoginCompleteCmd().SendAsync(ZoneConnection);
        }

        public void UpdateCharData(ZoneCharacterDataManager.CharClientDataType type)
        {
            ClientLogger.Debug($"Got UpdateCharData - {type}");
            _charClientDataStatus.Set(type);

            UpdateCharOption();

            if (_charClientDataStatus.ReceivedAll)
            {
                _ = UpdateStateAsync(ZoneServiceTrigger.ZST_LOGIN_OK);
            }
        }

        private void UpdateCharOption()
        {
            if (ActiveCharacter == null) return;

            if (ActiveCharacter.ShortcutData == null && ClientSessionData.ShortCutDatas != null)
            {
                ActiveCharacter.ShortcutData = ClientSessionData.ShortCutDatas;
            }
            // todo: other option data
        }
    }

    public class ZoneCharacterDataManager
    {
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
            CCDT_CHARTITLE = 12,
            CCDT_CHARGEDBUFF = 13,
            CCDT_GAME = 14,
            CCDT_COININFO = 15
        }

        public class CharClientDataStatus
        {
            private bool[] _allData = new bool[16];

            public bool ReceivedAll => _allData.All(a => a);

            public void Set(CharClientDataType type) => _allData[(byte)type] = true;

            public bool Get(CharClientDataType type) => _allData[(byte)type];

            public void Reset() => _allData = new bool[16];
        }
    }

    public enum ZoneServiceState
    {
        ZSS_DISCONNECTING,
        ZSS_DISCONNECTED,
        ZSS_CONNECTING,
        ZSS_CONNECTED,
        ZSS_LOGGED_IN,
        ZSS_LOGGED_IN_MAP,
    }

    public enum ZoneServiceTrigger
    {
        ZST_DISCONNECT,
        ZST_TRY_CONNECT,
        ZST_CONNECT_OK,
        ZST_CONNECT_FAIL,
        ZST_LOGIN_OK,
        ZST_LOGIN_FAIL,
        ZST_LOGIN_CHARACTER_FAIL,
        ZST_MAP_LOAD_OK,
    }
}
