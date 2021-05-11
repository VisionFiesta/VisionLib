using System.Linq;
using System.Threading.Tasks;
using Stateless;
using Vision.Game.Structs.Char;
using Vision.Game.Structs.Misc;
using Vision.Game.Structs.User;

namespace Vision.Client.Services
{
    public class WorldService : ClientServiceBase
    {
        private readonly StateMachine<WorldServiceState, WorldServiceTrigger> _worldStateMachine =
            new(WorldServiceState.WSS_DISCONNECTED, FiringMode.Queued);

        public WorldService(FiestaClient client) : base(client)
        {

            #region State machine setup
            _worldStateMachine.OnTransitioned(transition =>
            {
                if (transition.Source != transition.Destination)
                {
                    ClientLogger.Debug(
                        $"State Change - From {transition.Source} to {transition.Destination} because {transition.Trigger}");
                }
            });

            _worldStateMachine.OnUnhandledTrigger((state, trigger) =>
            {
                ClientLogger.Warning($"Got unhandled trigger for {state} : {trigger}");
            });

            _worldStateMachine.Configure(WorldServiceState.WSS_DISCONNECTING)
                .OnEntry(OnDisconnectingEntry)
                .OnEntryFrom(WorldServiceTrigger.WST_CONNECT_FAIL, () => ClientLogger.Error("World Connect FAIL"))
                .OnEntryFrom(WorldServiceTrigger.WST_LOGIN_FAIL, () => ClientLogger.Error("Map Login FAIL"))
                .OnEntryFrom(WorldServiceTrigger.WST_CHAR_LOGIN_FAIL, () => ClientLogger.Error("Character Login FAIL"))
                .OnEntryFrom(WorldServiceTrigger.WST_CONNECT_ZONE_FAIL, () => ClientLogger.Error("Zone Connect FAIL"))
                .Permit(WorldServiceTrigger.WST_DISCONNECT, WorldServiceState.WSS_DISCONNECTED);

            _worldStateMachine.Configure(WorldServiceState.WSS_DISCONNECTED)
                .OnEntry(OnDisconnectedEntry)
                .Permit(WorldServiceTrigger.WST_TRY_CONNECT, WorldServiceState.WSS_CONNECTING);

            _worldStateMachine.Configure(WorldServiceState.WSS_CONNECTING)
                .OnEntry(OnConnectingEntry)
                .Permit(WorldServiceTrigger.WST_DISCONNECT, WorldServiceState.WSS_DISCONNECTED)
                .Permit(WorldServiceTrigger.WST_CONNECT_OK, WorldServiceState.WSS_CONNECTED)
                .Permit(WorldServiceTrigger.WST_CONNECT_FAIL, WorldServiceState.WSS_DISCONNECTED);

            _worldStateMachine.Configure(WorldServiceState.WSS_CONNECTED)
                .OnEntry(OnConnectedEntry)
                .Permit(WorldServiceTrigger.WST_DISCONNECT, WorldServiceState.WSS_DISCONNECTED)
                .Permit(WorldServiceTrigger.WST_LOGIN_OK, WorldServiceState.WSS_LOGGED_IN)
                .Permit(WorldServiceTrigger.WST_LOGIN_FAIL, WorldServiceState.WSS_DISCONNECTING);

            _worldStateMachine.Configure(WorldServiceState.WSS_LOGGED_IN)
                .SubstateOf(WorldServiceState.WSS_CONNECTED)
                .OnEntryFrom(WorldServiceTrigger.WST_LOGIN_OK, OnLoggedInEntry)
                .OnExit(OnLoggedInExit)
                .PermitReentry(WorldServiceTrigger.WST_LEAVE_ZONE)
                .Permit(WorldServiceTrigger.WST_CHAR_SELECT, WorldServiceState.WSS_CHAR_SELECTED);

            _worldStateMachine.Configure(WorldServiceState.WSS_CHAR_SELECTED)
                .SubstateOf(WorldServiceState.WSS_LOGGED_IN)
                .OnEntry(OnCharSelectedEntry)
                .Permit(WorldServiceTrigger.WST_CHAR_LOGIN_OK, WorldServiceState.WSS_CHAR_LOGGED_IN)
                .Permit(WorldServiceTrigger.WST_CHAR_LOGIN_FAIL, WorldServiceState.WSS_LOGGED_IN);

            _worldStateMachine.Configure(WorldServiceState.WSS_CHAR_LOGGED_IN)
                .SubstateOf(WorldServiceState.WSS_LOGGED_IN)
                .OnEntry(OnCharLoggedInEntry)
                .Permit(WorldServiceTrigger.WST_TRY_JOIN_ZONE, WorldServiceState.WSS_ZONE_CONNECTING);

            _worldStateMachine.Configure(WorldServiceState.WSS_ZONE_CONNECTING)
                .SubstateOf(WorldServiceState.WSS_LOGGED_IN)
                .OnEntry(OnJoiningZoneEntry)
                .Permit(WorldServiceTrigger.WST_CONNECT_ZONE_OK, WorldServiceState.WSS_ZONE_CONNECTED)
                .Permit(WorldServiceTrigger.WST_CONNECT_ZONE_FAIL, WorldServiceState.WSS_LOGGED_IN);

            _worldStateMachine.Configure(WorldServiceState.WSS_ZONE_CONNECTED)
                .SubstateOf(WorldServiceState.WSS_LOGGED_IN)
                .OnEntry(OnZoneConnectedEntry)
                .Permit(WorldServiceTrigger.WST_LOGIN_ZONE_OK, WorldServiceState.WSS_ZONE_LOGGED_IN)
                .Permit(WorldServiceTrigger.WST_LOGIN_ZONE_FAIL, WorldServiceState.WSS_LOGGED_IN);

            _worldStateMachine.Configure(WorldServiceState.WSS_ZONE_LOGGED_IN)
                .SubstateOf(WorldServiceState.WSS_ZONE_CONNECTED)
                .OnEntry(OnZoneLoggedInEntry);

            // ClientLogger.Debug("WorldService StateMachine DOTGraph");
            // ClientLogger.Debug(UmlDotGraph.Format(_worldStateMachine.GetInfo()));

            #endregion

            ClientLogger.Info("Initialized");
        }

        public WorldServiceState GetState => _worldStateMachine.State;

        public void UpdateState(WorldServiceTrigger trigger)
        {
            _worldStateMachine.Deactivate();
            _worldStateMachine.Fire(trigger);
            _worldStateMachine.Activate();
        }

        public async Task UpdateStateAsync(WorldServiceTrigger trigger)
        {
            await _worldStateMachine.DeactivateAsync();
            await _worldStateMachine.FireAsync(trigger);
            await _worldStateMachine.ActivateAsync();
        }

        private void OnDisconnectingEntry()
        {
            if (WorldConnection.IsConnected) WorldConnection.Disconnect();
            ClientLogger.Info("Disconnecting...");
        }

        private void OnDisconnectedEntry()
        {
            ClientLogger.Info("Disconnected");
            Client.ClientSessionData.CleanupWorldData();
        }

        private void OnConnectingEntry()
        {
            ClientLogger.Info("Connecting...");
            var worldIP = Client.ClientSessionData.SelectedWorldEndPoint.Address.ToString();
            var port = (ushort)Client.ClientSessionData.SelectedWorldEndPoint.Port;
            WorldConnection.Connect(worldIP, port);
        }

        private void OnConnectedEntry()
        {
            ClientLogger.Info("Connected");
            var username = Client.ClientSessionData.ClientAccount.AccountName;
            var authBytes = Client.ClientSessionData.WorldAuthBytes;

            if (username != null && authBytes != null)
            {
                ClientLogger.Debug("Logging in");
                new NcUserLoginWorldReq(username, authBytes).Send(WorldConnection);
            }
            else
            {
                ClientLogger.Warning("Failed to log in to world - Connection data unavailable");
                UpdateState(WorldServiceTrigger.WST_DISCONNECT);
            }
        }

        private void OnLoggedInEntry()
        {
            ClientLogger.Info("Logged in");
            LoginService.UpdateState(LoginServiceTrigger.LST_JOIN_WORLD_OK);

            ClientLogger.Debug("Sending GameTimeReq");
            new NcMiscGameTimeReq().Send(WorldConnection);

            if (Client.UserData.IsBot)
            {
                UpdateState(WorldServiceTrigger.WST_CHAR_SELECT);
            }
        }

        private void OnLoggedInExit()
        {
            ClientLogger.Info("Logged out");
        }

        private void OnCharSelectedEntry()
        {
            var avatars = Client.ClientSessionData.ClientAccount.Avatars;
            var desiredCharacterName = Client.UserData.CharacterName;
            var matchingAvatar = avatars?.FirstOrDefault(a => a.CharName.Equals(desiredCharacterName));

            if (matchingAvatar != null)
            {
                ClientLogger.Info($"Connecting to zone with character {matchingAvatar.CharName}");
                var selected = Client.ClientSessionData.ClientAccount.SelectAvatar(matchingAvatar.CharNo);
                new NcCharLoginReq(matchingAvatar.Slot).Send(WorldConnection);
            }
            else
            {
                ClientLogger.Warning("Failed to connect to zone - Desired character unavailable");
            }
        }

        private void OnCharLoggedInEntry()
        {
            ClientLogger.Info("Character logged in");
            WorldService.UpdateState(WorldServiceTrigger.WST_TRY_JOIN_ZONE);
        }

        private void OnJoiningZoneEntry()
        {
            ClientLogger.Info("Zone connecting...");
            _ = ZoneService.UpdateState(ZoneServiceTrigger.ZST_TRY_CONNECT);
        }

        private void OnZoneConnectedEntry()
        {
            ClientLogger.Info("Zone connected");

        }

        private void OnZoneLoggedInEntry()
        {
            ClientLogger.Info("Logged in to Zone");
        }
    }

    public enum WorldServiceState
    {
        WSS_DISCONNECTING,
        WSS_DISCONNECTED,
        WSS_CONNECTING,
        WSS_CONNECTED,
        WSS_LOGGED_IN,
        WSS_CHAR_SELECTED,
        WSS_CHAR_LOGGED_IN,
        WSS_ZONE_CONNECTING,
        WSS_ZONE_CONNECTED,
        WSS_ZONE_LOGGED_IN
    }

    public enum WorldServiceTrigger
    {
        WST_DISCONNECT,
        WST_TRY_CONNECT,
        WST_CONNECT_OK,
        WST_CONNECT_FAIL,
        WST_LOGIN_OK,
        WST_LOGIN_FAIL,
        WST_CHAR_SELECT,
        WST_CHAR_LOGIN_OK,
        WST_CHAR_LOGIN_FAIL,
        WST_TRY_JOIN_ZONE,
        WST_CONNECT_ZONE_OK,
        WST_CONNECT_ZONE_FAIL,
        WST_LOGIN_ZONE_OK,
        WST_LOGIN_ZONE_FAIL,
        WST_LEAVE_ZONE
    }
}
