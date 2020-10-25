using System;
using System.Linq;
using System.Threading;
using System.Timers;
using Stateless;
using Vision.Core;
using Vision.Game.Structs.User;
using Timer = System.Timers.Timer;

namespace Vision.Client.Services
{
    public class LoginService : ClientServiceBase
    {
        private readonly StateMachine<LoginServiceState, LoginServiceTrigger> _loginStateMachine =
            new StateMachine<LoginServiceState, LoginServiceTrigger>(LoginServiceState.LSS_DISCONNECTED, FiringMode.Queued);

        private readonly Timer _worldListUpdateTimer = new Timer(ClientTimings.WorldListUpdatePeriod.TotalMilliseconds);

        public LoginService(FiestaClient client) : base(client)
        {

            #region State machine setup
            _loginStateMachine.OnTransitioned(transition =>
            {
                if (transition.Source != transition.Destination)
                {
                    ClientLogger.Debug(
                        $"State Change - From {transition.Source} to {transition.Destination} because {transition.Trigger}");
                }
            });

            _loginStateMachine.OnUnhandledTrigger((state, trigger) =>
            {
                ClientLogger.Warning($"Got unhandled trigger for {state} : {trigger}");
            });

            _loginStateMachine.Configure(LoginServiceState.LSS_DISCONNECTING)
                .OnEntry(OnDisconnectingEntry)
                .Permit(LoginServiceTrigger.LST_DISCONNECT, LoginServiceState.LSS_DISCONNECTED);

            _loginStateMachine.Configure(LoginServiceState.LSS_DISCONNECTED)
                .OnEntry(OnDisconnectedEntry)
                .Permit(LoginServiceTrigger.LST_TRY_CONNECT, LoginServiceState.LSS_CONNECTING)
                .Permit(LoginServiceTrigger.LST_LOGIN_FROM_WORLD, LoginServiceState.LSS_HAS_WORLDS);

            _loginStateMachine.Configure(LoginServiceState.LSS_CONNECTING)
                .OnEntry(OnConnectingEntry)
                .Permit(LoginServiceTrigger.LST_DISCONNECT, LoginServiceState.LSS_DISCONNECTED)
                .Permit(LoginServiceTrigger.LST_CONNECT_OK, LoginServiceState.LSS_CONNECTED)
                .Permit(LoginServiceTrigger.LST_CONNECT_FAIL, LoginServiceState.LSS_DISCONNECTED);

            _loginStateMachine.Configure(LoginServiceState.LSS_CONNECTED)
                .OnEntry(OnConnectedEntry)
                .Permit(LoginServiceTrigger.LST_DISCONNECT, LoginServiceState.LSS_DISCONNECTED)
                .Permit(LoginServiceTrigger.LST_VERIFY_OK, LoginServiceState.LSS_VERIFIED)
                .Permit(LoginServiceTrigger.LST_VERIFY_FAIL, LoginServiceState.LSS_DISCONNECTING);

            _loginStateMachine.Configure(LoginServiceState.LSS_VERIFIED)
                .SubstateOf(LoginServiceState.LSS_CONNECTED)
                .OnEntry(OnVerifiedEntry)
                .Permit(LoginServiceTrigger.LST_LOGIN_OK, LoginServiceState.LSS_LOGGED_IN)
                .Permit(LoginServiceTrigger.LST_GET_WORLDS_OK, LoginServiceState.LSS_HAS_WORLDS)
                .Permit(LoginServiceTrigger.LST_LOGIN_FAIL, LoginServiceState.LSS_DISCONNECTING);

            _loginStateMachine.Configure(LoginServiceState.LSS_LOGGED_IN)
                .SubstateOf(LoginServiceState.LSS_VERIFIED)
                .OnEntry(OnLoggedInEntry)
                .OnExit(OnLoggedInExit)
                .PermitReentry(LoginServiceTrigger.LST_GET_WORLDS_FAIL)
                .Permit(LoginServiceTrigger.LST_GET_WORLDS_OK, LoginServiceState.LSS_HAS_WORLDS);

            _loginStateMachine.Configure(LoginServiceState.LSS_HAS_WORLDS)
                .SubstateOf(LoginServiceState.LSS_LOGGED_IN)
                .OnEntry(OnHasWorldsEntry)
                .PermitReentry(LoginServiceTrigger.LST_SELECT_WORLD_FAIL)
                .Permit(LoginServiceTrigger.LST_TRY_SELECT_WORLD, LoginServiceState.LSS_SELECTING_WORLD);

            _loginStateMachine.Configure(LoginServiceState.LSS_SELECTING_WORLD)
                .SubstateOf(LoginServiceState.LSS_LOGGED_IN)
                .OnEntry(OnSelectingWorldEntry)
                .Permit(LoginServiceTrigger.LST_SELECT_WORLD_OK, LoginServiceState.LSS_JOINING_WORLD)
                .Permit(LoginServiceTrigger.LST_SELECT_WORLD_FAIL, LoginServiceState.LSS_HAS_WORLDS);

            _loginStateMachine.Configure(LoginServiceState.LSS_JOINING_WORLD)
                .SubstateOf(LoginServiceState.LSS_LOGGED_IN)
                .OnEntry(OnJoiningWorldEntry)
                .Permit(LoginServiceTrigger.LST_JOIN_WORLD_OK, LoginServiceState.LSS_IN_WORLD)
                .Permit(LoginServiceTrigger.LST_JOIN_WORLD_FAIL, LoginServiceState.LSS_HAS_WORLDS);

            _loginStateMachine.Configure(LoginServiceState.LSS_IN_WORLD)
                .OnEntry(OnInWorldEntry)
                .Permit(LoginServiceTrigger.LST_DISCONNECT, LoginServiceState.LSS_DISCONNECTING);
            #endregion

            _worldListUpdateTimer.AutoReset = true;
            _worldListUpdateTimer.Enabled = false;
            _worldListUpdateTimer.Elapsed += OnWorldsUpdateTick;

            ClientLogger.Info("Initialized");
        }

        #region State machine methods

        public void UpdateState(LoginServiceTrigger trigger)
        {
            _loginStateMachine.Deactivate();
            _loginStateMachine.Fire(trigger);
            _loginStateMachine.Activate();
        }

        private void OnDisconnectingEntry()
        {
            ClientLogger.Info("Disconnecting...");
            if (LoginConnection.IsConnected) LoginConnection.Disconnect();
        }

        private void OnDisconnectedEntry()
        {
            ClientLogger.Info("Disconnected");
            Client.ClientSessionData.CleanupLoginData();
        }

        private void OnConnectingEntry()
        {
            ClientLogger.Info("Connecting...");
            LoginConnection.Connect(UserData.LoginServerIP, UserData.LoginServerPort);
        }

        private void OnConnectedEntry()
        {
            ClientLogger.Info("Connected");
            ClientLogger.Debug("Sending VersionCheckReq");
            new NcUserClientVersionCheckReq(StaticClientData.VersionKey).Send(LoginConnection);
            // It doesn't seem to like this
            // new NcUserXTrapReq(StaticClientData.XTrapVersionHash).Send(LoginConnection);
        }

        private void OnVerifiedEntry()
        {
            Thread.Sleep(50);
            ClientLogger.Info("Verified");
            ClientLogger.Debug($"Sending User{Client.UserData.Region.ToPacketShortName()}LoginReq");

            switch (Client.UserData.Region)
            {
                case GameRegion.GR_NA:
                    new NcUserUSLoginReq(UserData.Username, UserData.Password).Send(LoginConnection);
                    break;
                case GameRegion.GR_DE:
                    new NcUserGERLoginReq(UserData.Username, UserData.Password).Send(LoginConnection);
                    break;
                case GameRegion.GR_TW:
                    throw new NotSupportedException("TW Login not supported yet!");
                case GameRegion.GR_KR:
                    throw new NotSupportedException("KR Login not supported yet!");
                case GameRegion.GR_JP:
                    throw new NotSupportedException("JP Login not supported yet!");
                case GameRegion.GR_CN:
                    throw new NotSupportedException("CN Login not supported yet!");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnLoggedInEntry()
        {
            ClientLogger.Info("Logged in");
            _worldListUpdateTimer.Enabled = true;
        }

        private void OnLoggedInExit()
        {
            ClientLogger.Debug("Disabling world list update timer");
            _worldListUpdateTimer.Enabled = false;
        }

        private void OnHasWorldsEntry()
        {
            ClientLogger.Info("Got world list");
            _worldListUpdateTimer.Enabled = true;

            if (Client.UserData.IsBot)
            {
                UpdateState(LoginServiceTrigger.LST_TRY_SELECT_WORLD);
            }
        }

        private void OnSelectingWorldEntry()
        {
            var worlds = Client.ClientSessionData.Worlds;
            var desiredWorldName = UserData.DesiredWorld.ToLower();
            var matchingWorld = worlds?.FirstOrDefault(w => w.WorldName.ToLower().Equals(desiredWorldName));

            if (matchingWorld != null)
            {
                ClientLogger.Debug($"Selecting world {matchingWorld.WorldName}");
                new NcUserWorldSelectReq(matchingWorld.WorldID).Send(LoginConnection);
            }
            else
            {
                ClientLogger.Warning("Failed to select world - Desired world unavailable");
            }
        }

        private void OnJoiningWorldEntry()
        {
            if (Client.ClientSessionData.SelectedWorldEndPoint != null && Client.ClientSessionData.WorldAuthBytes != null)
            {
                ClientLogger.Info("Joining world");
                WorldService.UpdateState(WorldServiceTrigger.WST_TRY_CONNECT);
            }
            else
            {
                ClientLogger.Warning("Failed to join world - Connection data unavailable");
            }
        }

        private void OnInWorldEntry()
        {
            ClientLogger.Info("In world, disconnecting");
            UpdateState(LoginServiceTrigger.LST_DISCONNECT);
        }

        #endregion

        private void OnWorldsUpdateTick(object sender, ElapsedEventArgs e)
        {
            if (LoginConnection.IsConnected)
            {
                ClientLogger.Debug("Sending WorldStatusReq");
                new NcUserWorldStatusReq().Send(LoginConnection);
            }
        }
    }

    public enum LoginServiceState
    {
        LSS_DISCONNECTING,
        LSS_DISCONNECTED,
        LSS_CONNECTING,
        LSS_CONNECTED,
        LSS_VERIFIED,
        LSS_LOGGED_IN,
        LSS_HAS_WORLDS,
        LSS_SELECTING_WORLD,
        LSS_JOINING_WORLD,
        LSS_IN_WORLD
    }

    public enum LoginServiceTrigger
    {
        LST_DISCONNECT,
        LST_TRY_CONNECT,
        LST_CONNECT_OK,
        LST_CONNECT_FAIL,
        LST_VERIFY_OK,
        LST_VERIFY_FAIL,
        LST_LOGIN_OK,
        LST_LOGIN_FAIL,
        LST_GET_WORLDS_OK,
        LST_GET_WORLDS_FAIL,
        LST_TRY_SELECT_WORLD,
        LST_SELECT_WORLD_OK,
        LST_SELECT_WORLD_FAIL,
        LST_JOIN_WORLD_OK,
        LST_JOIN_WORLD_FAIL,
        LST_LOGIN_FROM_WORLD
    }
}