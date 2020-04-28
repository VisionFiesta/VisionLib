using System;
using System.Collections.Generic;
using System.Drawing;
using Colorful;
using Console = Colorful.Console;

namespace VisionLib.Common.Logging
{
    public abstract class ColorfulConsoleLogger
    {
        private static string GetTimePrefix => DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff");
        private static Formatter GetTimeFormat => new Formatter($"[{GetTimePrefix}] ", Color.Aqua);

        private const string DefaultFormattingString = "{0}{1}{2}{3}";

        private byte _maxLogLevel;

        private readonly string _classPrefix;
        private readonly Color _classColor;

        private readonly object _ioLocker = new object();

        protected internal ColorfulConsoleLogger(string classPrefix, Color classColor)
        {
            _classPrefix = classPrefix;
            _classColor = classColor;
        }

        public void SetLogLevel(byte maxLevel)
        {
            _maxLogLevel = maxLevel;
        }

        public List<Formatter> GetMessagePrefixFormat(string messagePrefix, Color? prefixColor = null)
        {
            if (!prefixColor.HasValue) prefixColor = Color.White;

            return new List<Formatter>
            {
                GetTimeFormat,
                new Formatter($"[{_classPrefix}] ", _classColor),
                new Formatter($"[{messagePrefix}] ", prefixColor.Value)
            };
        }

        public Formatter[] GetFormattedMessage(string messagePrefix, string messageContent, Color? messageColor = null)
        {
            if (!messageColor.HasValue) messageColor = Color.White;

            var formatters = GetMessagePrefixFormat(messagePrefix, messageColor);
            formatters.Add(new Formatter(messageContent, messageColor.Value));
            return formatters.ToArray();
        }

        public void Write(byte messageLogLevel, string messagePrefix, string messageContent, Color? messageColor = null, params Formatter[] additionalPrefixes)
        {
            if (!ShouldLogByLogLevel(messageLogLevel)) return;

            var msg = GetFormattedMessage(messagePrefix, messageContent, messageColor);

            lock (_ioLocker) { Console.WriteFormatted(DefaultFormattingString, Color.White, msg); }
        }

        public void WriteLine(byte messageLogLevel, string messagePrefix, string messageContent, Color? messageColor = null)
        {
            if (!ShouldLogByLogLevel(messageLogLevel)) return;

            var msg = GetFormattedMessage(messagePrefix, messageContent, messageColor);

            lock (_ioLocker) { Console.WriteLineFormatted(DefaultFormattingString, Color.White, msg); }
        }

        public void WriteLineRaw(byte messageLogLevel, params Formatter[] messageContents)
        {
            if (!ShouldLogByLogLevel(messageLogLevel)) return;

            var formatStr = "";
            for (var i = 0; i < messageContents.Length; i++)
            {
                formatStr += $"{{{i}}}";
            }

            lock(_ioLocker) { Console.WriteLineFormatted(formatStr, Color.White, messageContents); }
        }

        private bool ShouldLogByLogLevel(byte messageLogLevel)
        {
            return ShouldLogPrecise(messageLogLevel) && messageLogLevel >= _maxLogLevel;
        }

        protected internal abstract bool ShouldLogPrecise(byte messageLogLevel);
    }
}
