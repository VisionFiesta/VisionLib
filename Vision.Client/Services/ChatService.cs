using System;
using System.Collections.Generic;
using System.Linq;
using Colorful;

using Vision.Core;
using Vision.Core.Enums;
using Vision.Core.Logging.Loggers;
using Vision.Game.Characters;
using Vision.Game.Content;
using Vision.Game.Enums;
using Vision.Game.Structs.Act;

namespace Vision.Client.Services
{

    public delegate void ChatSubscriberAction(ClientLogChatType type, string message, string sender, bool self);

    public sealed class ChatService : ClientServiceBase
    {
        private string _lastWhisperCharacterName = "";

        private readonly HashSet<ChatSubscriberAction> _chatSubscribers = new();

        public ChatService(FiestaClient client) : base(client)
        {
            ClientLogger.Info("Initialized");
        }

        public void AddChatSubscriber(ChatSubscriberAction chatSubscriberAction) => _chatSubscribers.Add(chatSubscriberAction);

        public void ReceiveChat(ClientLogChatType type, string message, int senderHandle)
        {
            if (senderHandle == ActiveCharacter?.Handle)
            {
                ReceiveChat(type, message, ActiveCharacter.Name);
            }
            else
            {
                var senderName = $"Unknown->Handle:{senderHandle}";
                var sender = ActiveCharacter?.VisibleObjects.FirstOrDefault(o => o.Handle == senderHandle);

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

        public void ReceiveChat(ClientLogChatType type, string message, string sender = "", string receiver = "")
        {
            var self = sender == ActiveAvatar?.CharName;
            LogChat(type, message, sender, self);

            if (type == ClientLogChatType.CLCT_WHISPER && receiver != "")
            {
                _lastWhisperCharacterName = receiver;
            }

            HandleChatCommands(type, message, sender);

            foreach (var chatSubscriberAction in _chatSubscribers)
            {
                // TODO: async-ify?
                chatSubscriberAction.Invoke(type, message, sender, self);
            }
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
                    MovementService.Jump();
                    break;
                case "follow":
                    // TODO: MovementService
                    break;
                default:
                    resp = "No comprende";
                    break;
            }

            if (resp != "") SendChatReq(type, resp, sender);
        }

        public void SendChatFromRawInput(ClientLogChatType type, string input)
        {
            // TODO: admin commands


            if (input.StartsWith("/") && type == ClientLogChatType.CLCT_NORMAL)
            {
                var commandSplit = input.Split(" ", 2);
                var commandPrefix = commandSplit[0].Replace("/", "");
                var possibleCommand = ChatCommandTypeExtensions.GetCommandFromPrefix(commandPrefix);

                if (possibleCommand.HasValue)
                {
                    var command = possibleCommand.Value;

                    var message = commandSplit[1];

                    switch (command)
                    {
                        case ChatCommandType.CCT_WHISPER_CHAT:
                            var whisperSplit = message.Split(" ", 2);
                            var whisperReceiver = whisperSplit[0];
                            var whisperMessage = whisperSplit[1];
                            SendChatReq(ClientLogChatType.CLCT_WHISPER, whisperMessage, whisperReceiver);
                            break;
                        case ChatCommandType.CCT_WHISPER_REPLY:
                            SendChatReq(ClientLogChatType.CLCT_WHISPER, message, _lastWhisperCharacterName);
                            break;
                        case ChatCommandType.CCT_SHOUT_CHAT:
                            SendChatReq(ClientLogChatType.CLCT_SHOUT, message);
                            break;
                        case ChatCommandType.CCT_PARTY_CHAT:
                            SendChatReq(ClientLogChatType.CLCT_PARTY, message);
                            break;
                        case ChatCommandType.CCT_PARTY_INVITE:
                            SendChatReq(ClientLogChatType.CLCT_PARTY, message);
                            break;
                        case ChatCommandType.CCT_ACADEMY_CHAT:
                            SendChatReq(ClientLogChatType.CLCT_ACADEMY, message);
                            break;
                        case ChatCommandType.CCT_GUILD_CHAT:
                            SendChatReq(ClientLogChatType.CLCT_GUILD, message);
                            break;
                        case ChatCommandType.CCT_EXPEDITION_NOTICE_CHAT:
                            SendChatReq(ClientLogChatType.CLCT_EXPEDITION, message, "NOTICE");
                            break;
                        case ChatCommandType.CCT_EXPEDITION_CHAT:
                            SendChatReq(ClientLogChatType.CLCT_EXPEDITION, message);
                            break;
                    }
                }
                else
                {
                    ClientLogger.Info("Invalid command.");
                }
            }
            else
            {
                SendChatReq(ClientLogChatType.CLCT_NORMAL, input);
            }
        }

        public void SendChatReq(ClientLogChatType type, string message, string receiver = "")
        {
            //TODO: split messages longer than 255
            switch (type)
            {
                case ClientLogChatType.CLCT_NORMAL:
                    var chatReq = new NcActChatReq(message);
                    chatReq.Send(ZoneConnection);
                    break;
                case ClientLogChatType.CLCT_SHOUT:
                    var shoutReq = new NcActShoutCmd(message);
                    shoutReq.Send(ZoneConnection);
                    break;
                case ClientLogChatType.CLCT_WHISPER:
                    if (string.IsNullOrEmpty(receiver)) return;
                    var whisperReq = new NcActWhisperReq(receiver, message);
                    whisperReq.Send(ZoneConnection);
                    _lastWhisperCharacterName = receiver;
                    break;
                case ClientLogChatType.CLCT_PARTY:
                    var partyReq = new NcActPartyChatReq(message);
                    partyReq.Send(ZoneConnection);
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
                case ClientLogChatType.CLCT_EXPEDITION:
                    if (receiver == "NOTICE")
                    {

                    }
                    else
                    {

                    }
                    // TODO: ExpeditionService
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public void GetWhisperSuccessAck(ClientLogChatType type, string message, string receiver = "") =>
            ReceiveChat(type, message, ActiveAvatar?.CharName, receiver);

        private void LogChat(ClientLogChatType type, string message, string sender, bool isSelf = false)
        {
            const ClientLogLevel level = ClientLogLevel.CLL_GAME;

            var messageColor = type.ToColor();

            var prefixFormatter = ClientLogger.GetMessagePrefixFormat("Chat");

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

            ClientLogger.WriteLineRawFormatted((byte)level, prefixFormatter.ToArray());
        }
    }
}
