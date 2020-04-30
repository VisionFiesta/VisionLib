using System;
using System.IO;
using System.Reflection;
using Vision.Core.Common.Extensions;

namespace Vision.Core.Common.Logging
{
    public class FileLogger : ConsoleLogger
    {
        private readonly string BaseDirectory = "Logs";

		/// <summary>
		/// Returns the path where the logs will be saved.
		/// </summary>
		public string LogDirectory { get; private set; }

		private readonly object _fileLock = new object();

		protected internal byte MaxFileLogLevel = byte.MaxValue;

		public FileLogger(string directory)
		{
			LogDirectory = Path.Combine(BaseDirectory, directory.ToEscapedString());

			if (!Directory.Exists(LogDirectory))
			{
				Directory.CreateDirectory(LogDirectory);
			}
		}

		public void SetFileLogLevel(LogLevel logLevel) => MaxFileLogLevel = (byte)logLevel;

		public void Write(LogType logType, LogLevel logSubType, string message, params object[] args)
		{
			try
			{
				var callingProcess = Assembly.GetEntryAssembly().GetName().Name;
				var filePath = $"{LogDirectory}{callingProcess}_{logType}_{DateTime.Now:MM_dd_yyyy_HH}.txt";
				var msg = ($"[{DateTime.Now}][{LogTypeName}][{logSubType}] {string.Format(message, args)}");;

				if ((byte)logSubType <= ConsoleLogLevel)
				{
					WriteLine(logType, logSubType, msg);
				}

				lock (_fileLock)
				{
					using (var tw = TextWriter.Synchronized(File.AppendText(filePath)))
					{
						if ((byte)logSubType <= MaxFileLogLevel)
						{
							tw.WriteLine(msg);
						}
					}
				}
			}
			catch (Exception ex)
			{
				WriteLine(LogType.EngineLog, LogLevel.Error, $"Exception while writing log:\n {ex}");
			}
		}
	}
}
