using UnityEngine;

namespace Mistral.AI.Logger
{
	public static class MistralLogger
	{
		public static void LogError(string message, Object context = null)
		{
			string formattedMessage = FormatMessage("ERROR", message);
			if (context != null)
				Debug.LogError(formattedMessage, context);
			else
				Debug.LogError(formattedMessage);
		}

		public static void LogWarning(string message, Object context = null)
		{
			string formattedMessage = FormatMessage("WARNING", message);
			if (context != null)
				Debug.LogWarning(formattedMessage, context);
			else
				Debug.LogWarning(formattedMessage);
		}

		public static void Log(string message, Object context = null)
		{
			string formattedMessage = FormatMessage("INFO", message);
			if (context != null)
				Debug.Log(formattedMessage, context);
			else
				Debug.Log(formattedMessage);
		}

		private static string FormatMessage(string level, string message)
		{
			string colorCode;

			switch (level)
			{
				case "ERROR":
					colorCode = "#FF4C4C";
					break;
				case "WARNING":
					colorCode = "#FFC107";
					break;
				case "INFO":
				default:
					colorCode = "#4CAF50";
					break;
			}

			return $"<color={colorCode}>[{level}]</color> {message}";
		}
	}
}