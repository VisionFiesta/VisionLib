using System;
using System.Collections.Generic;
using System.Drawing;
using Colorful;
using Console = Colorful.Console;
// ReSharper disable InconsistentlySynchronizedField

namespace Vision.Core.Logging
{
    public abstract class ColorfulConsoleLogger
    {
        private static string GetTimePrefix => DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff");
        private static Formatter GetTimeFormat => new Formatter($"[{GetTimePrefix}] ", Color.Aqua);

        private const string DefaultFormattingString = "{0}{1}{2}{3}";

        protected byte MaxLogLevel;

        public readonly string BasePrefix;
        public readonly Color BaseColor;

        private readonly object _ioLocker = new object();

        protected internal ColorfulConsoleLogger(string basePrefix, Color baseColor)
        {
            BasePrefix = basePrefix;
            BaseColor = baseColor;
        }

        public void SetLogLevel(byte maxLevel)
        {
            MaxLogLevel = maxLevel;
        }

        public List<Formatter> GetMessagePrefixFormat(string messagePrefix, Color? prefixColor = null)
        {
            if (!prefixColor.HasValue) prefixColor = BaseColor;

            return new List<Formatter>
            {
                GetTimeFormat,
                new Formatter($"[{BasePrefix}] ", BaseColor),
                new Formatter($"[{messagePrefix}] ", prefixColor.Value)
            };
        }

        public Formatter[] GetFormattedMessage(string messagePrefix, string messageContent, Color? messageColor = null)
        {
            if (!messageColor.HasValue) messageColor = BaseColor;

            var formatters = GetMessagePrefixFormat(messagePrefix, messageColor);
            formatters.Add(new Formatter(messageContent, messageColor.Value));
            return formatters.ToArray();
        }

        public void Write(byte messageLogLevel, string messagePrefix, string messageContent, Color? messageColor = null, params Formatter[] additionalPrefixes)
        {
            if (!ShouldLogByLogLevel(messageLogLevel)) return;

            var msg = GetFormattedMessage(messagePrefix, messageContent, messageColor);

            lock (_ioLocker) { Console.WriteFormatted(DefaultFormattingString, BaseColor, msg); }
        }

        public void WriteLine(byte messageLogLevel, string messagePrefix, string messageContent, Color? messageColor = null)
        {
            if (!ShouldLogByLogLevel(messageLogLevel)) return;

            var msg = GetFormattedMessage(messagePrefix, messageContent, messageColor);

            lock (_ioLocker) { Console.WriteLineFormatted(DefaultFormattingString, BaseColor, msg); }
        }

        public void WriteRaw(string str)
        {
            lock (_ioLocker)
            {
                Console.Write(str);
            }
        }

        public void WriteRaw(byte messageLogLevel, string message, Color? color = null)
        {
            if (!ShouldLogByLogLevel(messageLogLevel)) return;

            if (!color.HasValue) color = BaseColor;

            lock (_ioLocker) { Console.Write(message, color.Value); }
        }

        public void WriteLineRaw(byte messageLogLevel, string message, Color? color = null)
        {
            if (!ShouldLogByLogLevel(messageLogLevel)) return;

            if (!color.HasValue) color = BaseColor;

            lock (_ioLocker) { Console.WriteLine(message, color.Value); }
        }

        public void WriteRawFormatted(byte messageLogLevel, params Formatter[] messageContents)
        {
            if (!ShouldLogByLogLevel(messageLogLevel)) return;

            var formatStr = "";
            for (var i = 0; i < messageContents.Length; i++)
            {
                formatStr += $"{{{i}}}";
            }

            lock (_ioLocker) { Console.WriteFormatted(formatStr, BaseColor, messageContents); }
        }

        public void WriteLineRawFormatted(byte messageLogLevel, params Formatter[] messageContents)
        {
            if (!ShouldLogByLogLevel(messageLogLevel)) return;

            var formatStr = "";
            for (var i = 0; i < messageContents.Length; i++)
            {
                formatStr += $"{{{i}}}";
            }

            lock (_ioLocker) { Console.WriteLineFormatted(formatStr, BaseColor, messageContents); }
        }

        private bool ShouldLogByLogLevel(byte messageLogLevel)
        {
            return ShouldLogPrecise(messageLogLevel) && messageLogLevel >= MaxLogLevel;
        }

        protected internal abstract bool ShouldLogPrecise(byte messageLogLevel);
    }
}
