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
        private static Formatter GetTimeFormat => new($"[{GetTimePrefix}] ", Color.Aqua);

        private const string DefaultFormattingString = "{0}{1}{2}{3}";

        public readonly string BasePrefix;
        public readonly string OwnerClassName;
        public readonly Color BaseColor;

        private readonly object _ioLocker = new();

        protected internal ColorfulConsoleLogger(string basePrefix, string className, Color baseColor)
        {
            BasePrefix = basePrefix;
            OwnerClassName = className.Replace("`1", "");
            BaseColor = baseColor;
        }

        public List<Formatter> GetMessagePrefixFormat(string messagePrefix, Color? prefixColor = null)
        {
            prefixColor ??= BaseColor;

            var ownerClassPrefix = OwnerClassName.Equals("") ? "" : $"[{OwnerClassName}] ";
            var fullBasePrefix = $"[{BasePrefix}] ";

            return new List<Formatter>
            {
                GetTimeFormat,
                new(fullBasePrefix, BaseColor),
                new($"{ownerClassPrefix}[{messagePrefix}] ", prefixColor.Value)
            };
        }

        public Formatter[] GetFormattedMessage(string messagePrefix, string messageContent, Color? messageColor = null)
        {
            messageColor ??= BaseColor;

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

        public void WriteRaw(byte messageLogLevel, string message, Color? color = null)
        {
            if (!ShouldLogByLogLevel(messageLogLevel)) return;

            color ??= BaseColor;

            lock (_ioLocker) { Console.Write(message, color.Value); }
        }

        public void WriteLineRaw(byte messageLogLevel, string message, Color? color = null)
        {
            if (!ShouldLogByLogLevel(messageLogLevel)) return;

            color ??= BaseColor;

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
            return ShouldLogPrecise(messageLogLevel) && messageLogLevel >= GetLogLevel();
        }

        protected internal abstract byte GetLogLevel();

        protected internal abstract bool ShouldLogPrecise(byte messageLogLevel);
    }
}
