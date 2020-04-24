using System;
using System.Drawing;
using Console = Colorful.Console;

namespace VisionLib.Common.Logging
{
	public class ConsoleLogger
	{
		protected internal byte ConsoleLogLevel = byte.MaxValue;

		protected virtual string LogTypeName { get; }

		internal object IOLocker;

		public ConsoleLogger()
		{
			IOLocker = new object();
		}

		public void SetConsoleLevel(LogLevel logLevel)
		{
			ConsoleLogLevel = (byte)logLevel;
		}

		public void WriteLine(LogType logType, LogLevel logLevel, string message)
		{
			if ((byte)logLevel > ConsoleLogLevel) return;
			lock (IOLocker)
			{
				if (!ConsoleColors.GetColor(logType, logLevel, out var pColor)) return;

				Console.ForegroundColor = pColor;
				Console.WriteLine("\r" + message);
				Console.ResetColor();
			}
		}

		public void WriteLine(LogType logType, LogLevel logLevel, string message, params object[] args)
        {
            var levelByte = (byte) logLevel;
			if (levelByte < ConsoleLogLevel) return;
			lock (IOLocker)
			{
				if (!ConsoleColors.GetColor(logType, logLevel, out var pColor)) return;
				var msg = string.Format($"[{logType}][{logLevel}] {string.Format(message, args)}");

				Console.ForegroundColor = pColor;
				Console.WriteLine(msg);
				Console.ResetColor();
			}
		}

		public void WriteConsoleProgressBar(string text, params object[] args)
		{
			lock (IOLocker)
			{
				Console.ForegroundColor = Color.Green;
				Console.WriteLine(text, args);
				Console.ResetColor();
			}
		}

		public void WriteException(LogType logType, Exception exception, string prependMessage, params object[] args)
		{
			WriteLine(logType, LogLevel.Error, string.Format("{0}{1}{1}{1}{2}{1}{1}{3}{1}{1}{1}", prependMessage, Environment.NewLine, exception.Message, exception.StackTrace), args);
		}
	}
}
