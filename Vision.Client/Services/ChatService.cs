using Colorful;

using Vision.Client.Networking;
using Vision.Core;
using Vision.Core.Logging.Loggers;
using Vision.Core.Networking;
using Vision.Core.Networking.Packet;
using Vision.Game.Characters;
using Vision.Game.Content;
using Vision.Game.Structs.Act;

namespace Vision.Client.Services
{
    public sealed class ChatService
    {
        private static readonly ClientLog Logger = new ClientLog(typeof(ChatService));

        private readonly FiestaClient _client;
        private Character ActiveCharacter => _client.ClientSessionData.ClientAccount.ActiveCharacter;
        private NetClientConnection WorldClient => _client.WorldClient;
        private NetClientConnection ZoneClient => _client.ZoneClient;

        public ChatService(FiestaClient client)
        {
            _client = client;
            Logger.Info("Initialized");
        }

        public void ReceiveChat(ClientLogChatType type, string message, int senderHandle)
        {
            if (senderHandle == ActiveCharacter.Handle)
            {
                ReceiveChat(type, message, ActiveCharacter.Name);
            }
            else
            {
                var senderName = $"Unknown->Handle:{senderHandle}";
                var sender = ActiveCharacter.VisibleObjects.First(o => o.Handle == senderHandle);

                if (sender != null)
                {
                    switch (sender)
                    {
                        case Character character:
                            senderName = character.Name;
                            break;
                        case Mob mob:
                            var hasMobInfo = MobInfo.GetMobInfoById(mob.MobID, out var mobInfo);
                            senderName = hasMobInfo ? $"Mob: {mobInfo.Name}" : $"MobID: {mob.MobID}";
                            break;
                    }
                }

                ReceiveChat(type, message, senderName);
            }
        }

        public void ReceiveChat(ClientLogChatType type, string message, string sender)
        {
            LogChat(type, message, sender, sender == ActiveCharacter.Name);
            HandleChatCommands(type, message, sender);
        }

        private void HandleChatCommands(ClientLogChatType type, string message, string sender)
        {
            if (sender != "Life_Restorer") return;
            if (!message.StartsWith("svt")) return;

            string resp = "";
            switch (message.ToLower().Replace("svt", "").Trim())
            {
                case "hello":
                    resp = "Heya, Life!";
                    break;
                case "jump":
                    // TODO: ClientMovementService
                    new NetPacket(NetCommand.NC_ACT_JUMP_CMD).Send(ZoneClient);
                    break;
                case "follow":
                    // TODO: ClientMovementService
                    break;
                default:
                    resp = "No comprende";
                    break;
            }

            if (resp != "") SendChatReq(type, resp, sender);
        }
        
        public void SendChatReq(ClientLogChatType type, string message, string receiver = "")
        {
            switch (type)
            {
                case ClientLogChatType.CLCT_NORMAL:
                    var chatReq = new NcActChatReq(message);
                    chatReq.Send(ZoneClient);
                    break;
                case ClientLogChatType.CLCT_SHOUT:
                    var shoutReq = new NcActShoutCmd(message);
                    shoutReq.Send(ZoneClient);
                    break;
                case ClientLogChatType.CLCT_WHISPER:
                    if (string.IsNullOrEmpty(receiver)) return;
                    var whispReq = new NcActWhisperReq();
                    whispReq.Send(ZoneClient);
                    break;
                case ClientLogChatType.CLCT_PARTY:
                    var partyReq = new NcActPartyChatReq();
                    partyReq.Send(ZoneClient);
                    break;
                case ClientLogChatType.CLCT_ACADEMY:
                    // TODO: GuildService
                    break;
                case ClientLogChatType.CLCT_GUILD:
                    // TODO: GuildService
                    break;
                case ClientLogChatType.CLCT_ROAR:
                    // TODO: InventoryService
                    break;
            }
        }

        public void SendChatAck(ClientLogChatType type, string message, string receiver = "")
        {
            LogChat(type, message, receiver, true);
        }

        private static void LogChat(ClientLogChatType type, string message, string sender, bool isSelf = false)
        {
            const ClientLogLevel level = ClientLogLevel.CLL_GAME;

            var messageColor = type.ToColor();

            var prefixFormatter = Logger.GetMessagePrefixFormat("Chat");


            if (sender != "")
            {
                var senderColor = messageColor;

                if (!(type == ClientLogChatType.CLCT_ROAR || isSelf))
                {
                    senderColor = FiestaColors.ChatUsernameColor;
                }

                if (type == ClientLogChatType.CLCT_WHISPER)
                {
                    sender = isSelf ? $"To {sender}" : $"From {sender}";
                }

                prefixFormatter.Add(new Formatter($"[{sender}]", senderColor));
            }

            prefixFormatter.Add(new Formatter($"", messageColor));
            prefixFormatter.Add(new Formatter($" : {message}", messageColor));

            Logger.WriteLineRawFormatted((byte) level, prefixFormatter.ToArray());
        }
    }
}
