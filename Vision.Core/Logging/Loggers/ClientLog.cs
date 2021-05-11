using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Colorful;
using Vision.Core.Enums;
using Vision.Core.Extensions;

namespace Vision.Core.Logging.Loggers
{
    public sealed class ClientLog : ColorfulConsoleLogger
    {
        private const string LoggerPrefix = "ClientLog";
        private static readonly Color LoggerColor = Color.CornflowerBlue;

        private static ClientLogLevel _logLevel;

        public ClientLog(MemberInfo ownerClass) : base(LoggerPrefix, ownerClass.Name, LoggerColor) {}

        private static List<byte> _preciseLogLevels = new(EnumExtensions.GetValuesCasted<ClientLogLevel, byte>());

        public static void SetPreciseLogLevels(params ClientLogLevel[] levels) => _preciseLogLevels = new List<byte>(levels.Cast<byte>());

        public static void SetLogLevel(ClientLogLevel maxLogLevel) => _logLevel = maxLogLevel;

        public static bool ShowChat { get; set; } = true;
        public static bool ShowAnnounce { get; set; } = true;

        public static List<ClientLogChatType> AllowedChatTypes = new(EnumExtensions.GetValues<ClientLogChatType>());

        public static void SetChatFilter(params ClientLogChatType[] types) => AllowedChatTypes = new List<ClientLogChatType>(types);

        public void Debug(string message) => WriteLine(ClientLogLevel.CLL_DEBUG, message);

        public void Error(string message) => WriteLine(ClientLogLevel.CLL_ERROR, message);

        public void Warning(string message) => WriteLine(ClientLogLevel.CLL_WARNING, message);

        public void Info(string message) => WriteLine(ClientLogLevel.CLL_INFO, message);

        // TODO: item link?
        public void Chat(ClientLogChatType type, string sender, string message, bool isSelf = false)
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

                var prefixFormatter = GetMessagePrefixFormat(prefix, messageColor);
                prefixFormatter.Add(new Formatter($"[{sender}]", senderColor));
                prefixFormatter.Add(new Formatter($" : {message}", messageColor));

                WriteLineRawFormatted((byte)level, prefixFormatter.ToArray());
            }
            else
            {
                var prefixFormatter = GetMessagePrefixFormat(level.ToName(), level.ToColor());
                prefixFormatter.Add(new Formatter(message, messageColor));

                WriteLineRawFormatted((byte)level, prefixFormatter.ToArray());
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

        private void WriteLine(ClientLogLevel level, string message) => WriteLine((byte)level, level.ToName(), message, level.ToColor());

        protected internal override byte GetLogLevel() => (byte) _logLevel;

        protected internal override bool ShouldLogPrecise(byte messageLogLevel) => _preciseLogLevels.Contains(messageLogLevel);
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
}
