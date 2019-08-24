using System;
using System.IO;
using System.Text;

namespace StarCitizen_XML_to_JSON
{
	class Logger
	{
		private static StringBuilder sb = new StringBuilder();
		private static string date_format = "yyy-MM-dd HH:mm:ss.fff";

		/// <summary>
		/// Print DEFAULT log
		/// </summary>
		/// <param name="message"></param>
		/// <param name="end"></param>
		public static void Log(string message, char end = '\n')
		{
			string line = $"[+] {message}{end}";
			Console.Write(line);
			sb.Append(DateTime.Now.ToString($"[{date_format}] ") + line);
		}
		/// <summary>
		/// Print INFO log
		/// </summary>
		/// <param name="message"></param>
		/// <param name="end"></param>
		public static void LogInfo(string message, char end = '\n')
		{
			string line = $"[i] {message}{end}";
			Console.Write(line);
			sb.Append(DateTime.Now.ToString($"[{date_format}] ") + line);
		}

		/// <summary>
		/// Print log without prefix
		/// </summary>
		/// <param name="message"></param>
		/// <param name="end"></param>
		public static void LogEmpty(string message, char end = '\n')
		{
			string line = $"{message}{end}";
			Console.Write(line);
			sb.Append(DateTime.Now.ToString($"[{date_format}] ") + line);
		}

		/// <summary>
		/// Print ERROR log
		/// </summary>
		/// <param name="message"></param>
		/// <param name="end"></param>
		public static void LogError(string message, char end = '\n')
		{
			string line = $"[-] {message}{end}";
			Console.Write(line);
			sb.Append(DateTime.Now.ToString($"[{date_format}] ") + line);
		}

		/// <summary>
		/// Print WARNING log
		/// </summary>
		/// <param name="message"></param>
		/// <param name="end"></param>
		public static void LogWarning(string message, char end = '\n')
		{
			string line = $"[!] {message}{end}";
			Console.Write(line);
			sb.Append(DateTime.Now.ToString($"[{date_format}] ") + line);
		}

		/// <summary>
		/// Print DEBUG log
		/// </summary>
		/// <param name="message"></param>
		/// <param name="end"></param>
		public static void LogDebug(string message, char end = '\n')
		{
#if RELEASE
			return;
#endif
			string line = $"[D] {message}{end}";

			Console.Write(line);
			sb.Append(DateTime.Now.ToString($"[{date_format}] ") + line);
		}

		/// <summary>
		/// Append all logs to the file
		/// </summary>
		public static void WriteLog()
		{
			string filename = DateTime.Now.ToString("dd-MM-yyyy") + "_log.txt";

			// write the logs to the file
			using (StreamWriter writer = new StreamWriter(filename, true, Encoding.UTF8))
			{
				writer.Write(sb.ToString());
			}

			// clear the buffer
			sb.Clear();
		}
	}
}
