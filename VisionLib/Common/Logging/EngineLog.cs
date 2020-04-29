using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using VisionLib.Common.Extensions;

namespace VisionLib.Common.Logging
{
    public sealed class EngineLog : ColorfulConsoleLogger
    {
        private static EngineLog _instance;
        private static EngineLog Instance => _instance ?? (_instance = new EngineLog());

        internal EngineLog() : base("EngineLog", Color.Orange) { }

        private static List<byte> _preciseLogLevels = new List<byte>(EnumExtensions.GetValuesCasted<EngineLogLevel, byte>());

        public static void SetPreciseLogLevels(params EngineLogLevel[] levels)
        {
            _preciseLogLevels = new List<byte>(levels.Cast<byte>());
        }

        public static void SetLogLevel(EngineLogLevel maxLogLevel)
        {
            Instance.SetLogLevel((byte)maxLogLevel);
        }

        public static void Debug(string message) => WriteLine(EngineLogLevel.ELL_DEBUG, message);

        public static void Error(string prependMessage, Exception ex)
        {
            var msg = $"{prependMessage}:\n{ex.Message}\n{ex.StackTrace}";

            Instance.WriteLine((byte)EngineLogLevel.ELL_ERROR, "Exception", msg, Color.OrangeRed);
        }

        public static void Error(string message) => WriteLine(EngineLogLevel.ELL_ERROR, message);

        public static void Warning(string message) => WriteLine(EngineLogLevel.ELL_WARNING, message);

        public static void Info(string message) => WriteLine(EngineLogLevel.ELL_INFO, message);

        private static void WriteLine(EngineLogLevel level, string message)
        {
            Instance.WriteLine((byte)level, level.ToName(), message, level.ToColor());
        }

        protected internal override bool ShouldLogPrecise(byte messageLogLevel)
        {
            return _preciseLogLevels.Contains(messageLogLevel);
        }
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
