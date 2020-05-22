using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Vision.Core.Extensions;

namespace Vision.Core.Logging.Loggers
{
    public sealed class SocketLog : ColorfulConsoleLogger
    {
        private static SocketLog _instance;
        private static SocketLog Instance => _instance ??= new SocketLog();

        internal SocketLog() : base("SocketLog", Color.Purple) {}

        private static List<byte> _preciseLogLevels = new List<byte>(EnumExtensions.GetValuesCasted<SocketLogLevel, byte>());

        public static void SetPreciseLogLevels(params SocketLogLevel[] levels)
        {
            _preciseLogLevels = new List<byte>(levels.Cast<byte>());
        }

        public static void SetLogLevel(SocketLogLevel maxLogLevel)
        {
            Instance.SetLogLevel((byte)maxLogLevel);
        }

        public static void Debug(string message) => WriteLine(SocketLogLevel.SLL_DEBUG, message);

        public static void Unhandled(string message) => WriteLine(SocketLogLevel.SLL_UNHANDLED, message);

        public static void Error(string message) => WriteLine(SocketLogLevel.SLL_ERROR, message);

        public static void Warning(string message) => WriteLine(SocketLogLevel.SLL_WARNING, message);

        public static void Info(string message) => WriteLine(SocketLogLevel.SLL_INFO, message);

        private static void WriteLine(SocketLogLevel level, string message)
        {
            Instance.WriteLine((byte)level, level.ToName(), message, level.ToColor());
        }

        protected internal override bool ShouldLogPrecise(byte messageLogLevel)
        {
            return _preciseLogLevels.Contains(messageLogLevel);
        }
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
