using System;

namespace Vision.Core.Common.Logging
{
    public sealed class Log : FileLogger
    {
        private Log() : base("") {}

        private static Log _instance;
        private static Log Instance => _instance ?? (_instance = new Log());

        public static void SetConsoleLogLevel(LogLevel level) => Instance.SetConsoleLevel(level);
        public new static void SetFileLogLevel(LogLevel level) => ((FileLogger) Instance).SetFileLogLevel(level);

        public new static void Write(LogType type, LogLevel level, string message, params object[] args)
        {
            Instance.WriteLine(type, level, message, args);
        }

        public static void Write(LogType type, Exception exception, string prependMessage, params object[] args)
        {
            Instance.WriteException(type, exception, prependMessage, args);
        }
    }
}
