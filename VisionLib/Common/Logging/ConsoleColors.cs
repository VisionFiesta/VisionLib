using System.Collections.Generic;
using System.Drawing;

namespace VisionLib.Common.Logging
{
	public static class ConsoleColors
	{
		private static readonly Dictionary<LogLevel, Color> GameColors = new Dictionary<LogLevel, Color>
		{
			{LogLevel.Debug, Color.Magenta},
			{LogLevel.Startup, Color.Green},
			{LogLevel.Info, Color.Cyan},
			{LogLevel.Warning, Color.Yellow},
			{LogLevel.Error, Color.Red},
		};

		private static readonly Dictionary<LogLevel, Color> FileColors = new Dictionary<LogLevel, Color>
		{
			{LogLevel.Debug, Color.Magenta},
			{LogLevel.Startup, Color.Green},
			{LogLevel.Info, Color.Gray},
			{LogLevel.Warning, Color.Yellow},
			{LogLevel.Error, Color.Red},
		};

		private static readonly Dictionary<LogLevel, Color> SocketColors = new Dictionary<LogLevel, Color>
		{
			{LogLevel.Debug, Color.LightSkyBlue},
			{LogLevel.Startup, Color.LightBlue},
			{LogLevel.Info, Color.LightBlue},
			{LogLevel.Warning, Color.Blue},
			{LogLevel.Error, Color.DarkBlue},
		};

		private static readonly Dictionary<LogLevel, Color> EngineColors = new Dictionary<LogLevel, Color>
		{
			{LogLevel.Debug, Color.Magenta},
			{LogLevel.Info, Color.DarkGreen},
			{LogLevel.Startup, Color.Green},
			{LogLevel.Warning, Color.Yellow},
			{LogLevel.Error, Color.Red},
		};

		private static readonly Dictionary<LogLevel, Color> CommandColors = new Dictionary<LogLevel, Color>
		{
			{LogLevel.Debug, Color.Magenta},
			{LogLevel.Startup, Color.Green},
			{LogLevel.Info, Color.Gray},
			{LogLevel.Warning, Color.Yellow},
			{LogLevel.Error, Color.Red},
		};

		public static bool GetColor(LogType logType, LogLevel logLevel, out Color pColor)
		{
			switch (logType)
			{
				case LogType.EngineLog:
					return EngineColors.TryGetValue(logLevel, out pColor);
				case LogType.CommandLog:
					return CommandColors.TryGetValue(logLevel, out pColor);
				case LogType.FileLog:
					return FileColors.TryGetValue(logLevel, out pColor);
				case LogType.GameLog:
					return GameColors.TryGetValue(logLevel, out pColor);
				case LogType.SocketLog:
					return SocketColors.TryGetValue(logLevel, out pColor);
				default:
					pColor = Color.White;
					return true;
			}
		}
	}
}
