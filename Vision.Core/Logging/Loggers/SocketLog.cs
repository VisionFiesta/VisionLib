using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Vision.Core.Common.Extensions;

namespace Vision.Core.Common.Logging.Loggers
{
    public sealed class SocketLog : ColorfulConsoleLogger
    {
        private static SocketLog _instance;
        private static SocketLog Instance => _instance ?? (_instance = new SocketLog());

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
            switch (level)
            {
                case SocketLogLevel.SLL_DEBUG: return Color.BlueViolet;
                case SocketLogLevel.SLL_UNHANDLED: return Color.LightCoral;
                case SocketLogLevel.SLL_ERROR: return Color.Red;
                case SocketLogLevel.SLL_WARNING: return Color.DarkOrange;
                case SocketLogLevel.SLL_INFO: return Color.Chartreuse;
                default: return Color.White;
            }
        }

        public static string ToName(this SocketLogLevel level)
        {
            switch (level)
            {
                case SocketLogLevel.SLL_DEBUG: return "Debug";
                case SocketLogLevel.SLL_UNHANDLED: return "Unhandled";
                case SocketLogLevel.SLL_ERROR: return "Error";
                case SocketLogLevel.SLL_WARNING: return "Warning";
                case SocketLogLevel.SLL_INFO: return "Info";
                default: return "Unk";
            }
        }
    }
}
