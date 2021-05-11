using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Vision.Core.Extensions;

namespace Vision.Core.Logging.Loggers
{
    public sealed class SocketLog : ColorfulConsoleLogger
    {
        private const string LoggerName = "SocketLog";
        private static readonly Color LoggerColor = Color.Purple;

        private static SocketLogLevel _logLevel;

        public SocketLog(MemberInfo ownerClass) : base(LoggerName, ownerClass.Name, LoggerColor) {}

        private static List<byte> _preciseLogLevels = new(EnumExtensions.GetValuesCasted<SocketLogLevel, byte>());

        public static void SetPreciseLogLevels(params SocketLogLevel[] levels) => _preciseLogLevels = new List<byte>(levels.Cast<byte>());

        public static void SetLogLevel(SocketLogLevel maxLogLevel) => _logLevel = maxLogLevel;

        public  void Debug(string message) => WriteLine(SocketLogLevel.SLL_DEBUG, message);

        public  void Unhandled(string message) => WriteLine(SocketLogLevel.SLL_UNHANDLED, message);

        public  void Error(string message) => WriteLine(SocketLogLevel.SLL_ERROR, message);

        public  void Warning(string message) => WriteLine(SocketLogLevel.SLL_WARNING, message);

        public  void Info(string message) => WriteLine(SocketLogLevel.SLL_INFO, message);

        private void WriteLine(SocketLogLevel level, string message) => WriteLine((byte)level, level.ToName(), message, level.ToColor());

        protected internal override byte GetLogLevel() => (byte) _logLevel;

        protected internal override bool ShouldLogPrecise(byte messageLogLevel) => _preciseLogLevels.Contains(messageLogLevel);
    }

    public enum SocketLogLevel : byte
    {
        SLL_DEBUG = 0,
        SLL_UNHANDLED = 1,
        SLL_ERROR = 2,
        SLL_WARNING = 3,
        SLL_INFO = 4
    }

    public static class SocketLogLevelExtensions
    {
        public static Color ToColor(this SocketLogLevel level)
        {
            return level switch
            {
                SocketLogLevel.SLL_DEBUG => Color.BlueViolet,
                SocketLogLevel.SLL_UNHANDLED => Color.LightCoral,
                SocketLogLevel.SLL_ERROR => Color.Red,
                SocketLogLevel.SLL_WARNING => Color.DarkOrange,
                SocketLogLevel.SLL_INFO => Color.Chartreuse,
                _ => Color.White
            };
        }

        public static string ToName(this SocketLogLevel level)
        {
            return level switch
            {
                SocketLogLevel.SLL_DEBUG => "Debug",
                SocketLogLevel.SLL_UNHANDLED => "Unhandled",
                SocketLogLevel.SLL_ERROR => "Error",
                SocketLogLevel.SLL_WARNING => "Warning",
                SocketLogLevel.SLL_INFO => "Info",
                _ => "Unk"
            };
        }
    }
}
