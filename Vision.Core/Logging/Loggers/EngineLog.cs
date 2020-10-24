using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Colorful;
using Vision.Core.Extensions;
using Vision.Core.Logging.ProgressBar;

namespace Vision.Core.Logging.Loggers
{
    public sealed class EngineLog : ColorfulConsoleLogger
    {
        private const string LoggerPrefix = "EngineLog";
        private static readonly Color LoggerColor = Color.Orange;

        private static EngineLogLevel _logLevel;

        public EngineLog(MemberInfo ownerClass) : base(LoggerPrefix, ownerClass.Name, LoggerColor) {}

        private static List<byte> _preciseLogLevels = new List<byte>(EnumExtensions.GetValuesCasted<EngineLogLevel, byte>());

        public static void SetPreciseLogLevels(params EngineLogLevel[] levels) => _preciseLogLevels = new List<byte>(levels.Cast<byte>());

        public static void SetLogLevel(EngineLogLevel maxLogLevel) => _logLevel = maxLogLevel;

        public void Debug(string message) => WriteLine(EngineLogLevel.ELL_DEBUG, message);

        public void Error(string prependMessage, System.Exception ex)
            => WriteLine((byte)EngineLogLevel.ELL_ERROR, "Exception", $"{prependMessage}: {ex.Message}", Color.OrangeRed);

        public void Error(string message) => WriteLine(EngineLogLevel.ELL_ERROR, message);

        public void Warning(string message) => WriteLine(EngineLogLevel.ELL_WARNING, message);

        public void Info(string message) => WriteLine(EngineLogLevel.ELL_INFO, message);

        public AProgressBar CreateProgressBar(string prefix, Color? color = null)
        {
            color ??= BaseColor;

            const EngineLogLevel level = EngineLogLevel.ELL_INFO;
            var prefixFormatter = GetMessagePrefixFormat(level.ToName(), level.ToColor());
            prefixFormatter.Add(new Formatter(prefix + " -> ", color.Value));

            if (_logLevel == EngineLogLevel.ELL_DEBUG)
            {
                return new FancyProgressBar(prefixFormatter, (byte)level, this);
            }
            return new DummyProgressBar((byte)level, this);
        }

        public void WriteLine(EngineLogLevel level, string message) => WriteLine((byte)level, level.ToName(), message, level.ToColor());

        protected internal override byte GetLogLevel() => (byte) _logLevel;

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
            return level switch
            {
                EngineLogLevel.ELL_DEBUG => Color.BlueViolet,
                EngineLogLevel.ELL_ERROR => Color.Red,
                EngineLogLevel.ELL_WARNING => Color.DarkOrange,
                EngineLogLevel.ELL_INFO => Color.Chartreuse,
                _ => Color.White
            };
        }

        public static string ToName(this EngineLogLevel level)
        {
            return level switch
            {
                EngineLogLevel.ELL_DEBUG => "Debug",
                EngineLogLevel.ELL_ERROR => "Error",
                EngineLogLevel.ELL_WARNING => "Warning",
                EngineLogLevel.ELL_INFO => "Info",
                _ => "Unk"
            };
        }
    }
}
