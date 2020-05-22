﻿using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Colorful;
using Vision.Core.Extensions;

namespace Vision.Core.Logging.Loggers
{
    public sealed class ClientLog : ColorfulConsoleLogger
    {
        private static ClientLog _instance;
        public static ClientLog Instance => _instance ??= new ClientLog();

        internal ClientLog() : base("ClientLog", Color.CornflowerBlue) { }

        private static List<byte> _preciseLogLevels = new List<byte>(EnumExtensions.GetValuesCasted<ClientLogLevel, byte>());

        public static void SetPreciseLogLevels(params ClientLogLevel[] levels)
        {
            _preciseLogLevels = new List<byte>(levels.Cast<byte>());
        }

        public static void SetLogLevel(ClientLogLevel level)
        {
            Instance.SetLogLevel((byte)level);
        }

        public static bool ShowChat { get; set; } = true;
        public static bool ShowAnnounce { get; set; } = true;

        public static List<ClientLogChatType> AllowedChatTypes = new List<ClientLogChatType>(EnumExtensions.GetValues<ClientLogChatType>());

        public static void SetChatFilter(params ClientLogChatType[] types) => AllowedChatTypes = new List<ClientLogChatType>(types);

        public static void Debug(string message) => WriteLine(ClientLogLevel.CLL_DEBUG, message);

        public static void Error(string message) => WriteLine(ClientLogLevel.CLL_ERROR, message);

        public static void Warning(string message) => WriteLine(ClientLogLevel.CLL_WARNING, message);

        public static void Info(string message) => WriteLine(ClientLogLevel.CLL_INFO, message);

        // TODO: item link?
        public static void Chat(ClientLogChatType type, string sender, string message, bool isSelf = false)
        {
            if (!ShowChat) return;

            const ClientLogLevel level = ClientLogLevel.CLL_GAME;
            var prefix = $"{type.ToName()}Chat";

            var messageColor = type.ToColor();

            if (type == ClientLogChatType.CLCT_NORMAL && isSelf) messageColor = FiestaColors.ChatSelfNormalColor;

            if (sender != "")
            {
                var senderColor = type.ToColor();

                if (!(type == ClientLogChatType.CLCT_ROAR || isSelf))
                {
                    senderColor = FiestaColors.ChatUsernameColor;
                }

                if (type == ClientLogChatType.CLCT_WHISPER)
                {
                    sender = isSelf ? $"To {sender}" : $"From {sender}";
                }

                var prefixFormatter = Instance.GetMessagePrefixFormat(prefix, messageColor);
                prefixFormatter.Add(new Formatter($"[{sender}]", senderColor));
                prefixFormatter.Add(new Formatter($" : {message}", messageColor));

                Instance.WriteLineRawFormatted((byte)level, prefixFormatter.ToArray());
            }
            else
            {
                var prefixFormatter = Instance.GetMessagePrefixFormat(level.ToName(), level.ToColor());
                prefixFormatter.Add(new Formatter(message, messageColor));

                Instance.WriteLineRawFormatted((byte)level, prefixFormatter.ToArray());
            }
        }

        // TODO: move to ChatServices
        // public static void Announce(AnnounceType type, string message)
        // {
        //     // ROAR is considered chat
        //     if (type == AnnounceType.AT_ROAR)
        //     {
        //         // yes it really is this stupid
        //         var split = message.Split(':');
        //         var sender = split[0].TrimEnd(' ');
        //         var msg = split[1].TrimStart(' ');
        //         Chat(ClientLogChatType.CLCT_ROAR, sender, msg);
        //     }
        //     else if (ShowAnnounce)
        //     {
        //         const ClientLogLevel level = ClientLogLevel.CLL_INFO;
        //
        //         Instance.WriteLine((byte)level, "Announce", message, Color.LimeGreen);
        //     }
        // }

        private static void WriteLine(ClientLogLevel level, string message)
        {
            Instance.WriteLine((byte)level, level.ToName(), message, level.ToColor());
        }

        protected internal override bool ShouldLogPrecise(byte messageLogLevel)
        {
            return _preciseLogLevels.Contains(messageLogLevel);
        }
    }

    public enum ClientLogLevel : byte
    {
        CLL_DEBUG = 0,
        CLL_GAME = 1,
        CLL_ERROR = 2,
        CLL_WARNING = 3,
        CLL_INFO = 4
    }

    public static class ClientLogLevelExtensions
    {
        public static Color ToColor(this ClientLogLevel level)
        {
            return level switch
            {
                ClientLogLevel.CLL_DEBUG => Color.BlueViolet,
                ClientLogLevel.CLL_GAME => Color.LightSeaGreen,
                ClientLogLevel.CLL_ERROR => Color.Red,
                ClientLogLevel.CLL_WARNING => Color.DarkOrange,
                ClientLogLevel.CLL_INFO => Color.Chartreuse,
                _ => Color.White
            };
        }

        public static string ToName(this ClientLogLevel level)
        {
            return level switch
            {
                ClientLogLevel.CLL_DEBUG => "Debug",
                ClientLogLevel.CLL_GAME => "Game",
                ClientLogLevel.CLL_ERROR => "Error",
                ClientLogLevel.CLL_WARNING => "Warning",
                ClientLogLevel.CLL_INFO => "Info",
                _ => "Unk"
            };
        }
    }

    public enum ClientLogChatType
    {
        CLCT_NORMAL,
        CLCT_SHOUT,
        CLCT_WHISPER,
        CLCT_PARTY,
        CLCT_ACADEMY,
        CLCT_GUILD,
        CLCT_ROAR
    }

    public static class ClientChatTypeExtensions
    {
        public static Color ToColor(this ClientLogChatType level)
        {
            return level switch
            {
                ClientLogChatType.CLCT_NORMAL => Color.White,
                ClientLogChatType.CLCT_SHOUT => FiestaColors.ChatShoutColor,
                ClientLogChatType.CLCT_WHISPER => FiestaColors.ChatWhisperColor,
                ClientLogChatType.CLCT_PARTY => FiestaColors.ChatPartyColor,
                ClientLogChatType.CLCT_ACADEMY => FiestaColors.ChatAcademyColor,
                ClientLogChatType.CLCT_GUILD => FiestaColors.ChatGuildColor,
                ClientLogChatType.CLCT_ROAR => FiestaColors.ChatRoarColor,
                _ => Color.White
            };
        }

        public static string ToName(this ClientLogChatType level)
        {
            return level switch
            {
                ClientLogChatType.CLCT_NORMAL => "Normal",
                ClientLogChatType.CLCT_SHOUT => "Shout",
                ClientLogChatType.CLCT_WHISPER => "Whisper",
                ClientLogChatType.CLCT_PARTY => "Party",
                ClientLogChatType.CLCT_ACADEMY => "Academy",
                ClientLogChatType.CLCT_GUILD => "Guild",
                ClientLogChatType.CLCT_ROAR => "Roar",
                _ => "Unk"
            };
        }
    }
}
