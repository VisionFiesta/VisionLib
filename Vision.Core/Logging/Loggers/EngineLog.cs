using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Colorful;
using Vision.Core.Extensions;
using Vision.Core.Logging.ProgressBar;

namespace Vision.Core.Logging.Loggers
{
    public sealed class EngineLog : ColorfulConsoleLogger
    {
        private static EngineLog _instance;
        public static EngineLog Instance => _instance ?? (_instance = new EngineLog());

        internal EngineLog() : base("EngineLog", Color.Orange) { }

        private static List<byte> _preciseLogLevels = new List<byte>(EnumExtensions.GetValuesCasted<EngineLogLevel, byte>());

        public static void SetPreciseLogLevels(params EngineLogLevel[] levels) => _preciseLogLevels = new List<byte>(levels.Cast<byte>());

        public static void SetLogLevel(EngineLogLevel maxLogLevel) => Instance.SetLogLevel((byte)maxLogLevel);

        public static void Debug(string message) => WriteLine(EngineLogLevel.ELL_DEBUG, message);

        public static void Error(string prependMessage, System.Exception ex) 
            => Instance.WriteLine((byte)EngineLogLevel.ELL_ERROR, "Exception", $"{prependMessage}:\n{ex.Message}\n{ex.StackTrace}", Color.OrangeRed);

        public static void Error(string message) => WriteLine(EngineLogLevel.ELL_ERROR, message);

        public static void Warning(string message) => WriteLine(EngineLogLevel.ELL_WARNING, message);

        public static void Info(string message) => WriteLine(EngineLogLevel.ELL_INFO, message);

        public static AProgressBar CreateProgressBar(string prefix, Color? color = null)
        {
            if (!color.HasValue) color = Instance.BaseColor;

            const EngineLogLevel level = EngineLogLevel.ELL_INFO;
            var prefixFormatter = Instance.GetMessagePrefixFormat(level.ToName(), level.ToColor());
            prefixFormatter.Add(new Formatter(prefix + " -> ", color.Value));

            if ((EngineLogLevel) Instance.MaxLogLevel == EngineLogLevel.ELL_DEBUG)
            {
                return new FancyProgressBar(prefixFormatter, (byte)level, Instance);
            }
            return new DummyProgressBar((byte)level, Instance);
        }

        public static void WriteLine(EngineLogLevel level, string message) => Instance.WriteLine((byte)level, level.ToName(), message, level.ToColor());

        protected internal override bool ShouldLogPrecise(byte messageLogLevel) => _preciseLogLevels.Contains(messageLogLevel);
    }

    public enum EngineLogLevel : byte
    {
        ELL_DEBUG = 0,
        ELL_ERROR = 1,
        ELL_WARNING = 2,
        ELL_INFO = 3
    }

    public static class EngineLogLevelExtensions
    {
        public static Color ToColor(this EngineLogLevel level)
        {
            switch (level)
            {
                case EngineLogLevel.ELL_DEBUG: return Color.BlueViolet;
                case EngineLogLevel.ELL_ERROR: return Color.Red;
                case EngineLogLevel.ELL_WARNING: return Color.DarkOrange;
                case EngineLogLevel.ELL_INFO: return Color.Chartreuse;
                default: return Color.White;
            }
        }

        public static string ToName(this EngineLogLevel level)
        {
            switch (level)
            {
                case EngineLogLevel.ELL_DEBUG: return "Debug";
                case EngineLogLevel.ELL_ERROR: return "Error";
                case EngineLogLevel.ELL_WARNING: return "Warning";
                case EngineLogLevel.ELL_INFO: return "Info";
                default: return "Unk";
            }
        }
    }
}
