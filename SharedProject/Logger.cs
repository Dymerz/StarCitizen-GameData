using System;
using System.IO;
using System.Text;

namespace SharedProject
{
	class Logger
	{
		private static StringBuilder sb = new StringBuilder();
		private static string date_format = "yyy-MM-dd HH:mm:ss.fff";
		public static string filename = DateTime.Now.ToString("dd-MM-yyyy") + "_log.txt";

		public static bool debug = false;

		/// <summary>
		/// Print DEFAULT log
		/// </summary>
		/// <param name="message"></param>
		/// <param name="end"></param>
		public static void Log(string message, string end = "\n", string start = "", bool clear_line = false)
		{
			string line = $"{start}[+] {message}";

			if (clear_line)
				line += new String(' ', Console.WindowWidth - (line.Length + Console.CursorLeft)) + end;
			else
				line += end;

			Console.Write(line);
			sb.Append(DateTime.Now.ToString($"[{date_format}] ") + $"[+] {message}\n");
		 }
		/// <summary>
		/// Print INFO log
		/// </summary>
		/// <param name="message"></param>
		/// <param name="end"></param>
		public static void LogInfo(string message, string end = "\n", string start = "", bool clear_line = false)
		{
			string line = $"{start}[*] {message}";

			if (clear_line)
				line += new String(' ', Console.WindowWidth - (line.Length + Console.CursorLeft)) + end;
			else
				line += end;

			Console.Write(line);
			sb.Append(DateTime.Now.ToString($"[{date_format}] ") + $"[*] {message}\n");
		}

		/// <summary>
		/// Print ERROR log
		/// </summary>
		/// <param name="message"></param>
		/// <param name="end"></param>
		public static void LogError(string message, Exception exception = null, string end = "\n", string start = "", bool clear_line = false)
		{
			string line = $"{start}[-] {message}";

			if (clear_line)
				line += new String(' ', Console.WindowWidth - (line.Length + Console.CursorLeft)) + end;
			else
				line += end;

			Console.Write(line);
			sb.Append(DateTime.Now.ToString($"[{date_format}] ") + $"[-] {message}\n");

			if (exception != null)
			{
				sb.Append(exception.StackTrace + "\n");
				sb.Append(exception.Message + "\n");
			}
		}

		/// <summary>
		/// Print WARNING log
		/// </summary>
		/// <param name="message"></param>
		/// <param name="end"></param>
		public static void LogWarning(string message, string end = "\n", string start = "", bool clear_line = false)
		{
			string line = $"{start}[!] {message}";

			if (clear_line)
				line += new String(' ', Console.WindowWidth - (line.Length + Console.CursorLeft)) + end;
			else
				line += end;

			Console.Write(line);
			sb.Append(DateTime.Now.ToString($"[{date_format}] ") + $"[!] {message}\n");
		}

		/// <summary>
		/// Print DEBUG log
		/// </summary>
		/// <param name="message"></param>
		/// <param name="end"></param>
		public static void LogDebug(string message, string end = "\n", string start = "", bool clear_line = false)
		{
			string line = $"{start}[D] {message}";

			if (clear_line)
				line += new String(' ', Console.WindowWidth - (line.Length + Console.CursorLeft)) + end;
			else
				line += end;

			sb.Append(DateTime.Now.ToString($"[{date_format}] ") + $"[D] {message}\n");
#if RELEASE
			if (!debug)
				return;
#endif
			Console.Write(line);
		}

		/// <summary>
		/// Print log without prefix
		/// </summary>
		/// <param name="message"></param>
		/// <param name="end"></param>
		public static void LogEmpty(string message = "", string end = "\n", string start = "", bool clear_line = false)
		{
			string line = $"{start}{message}";

			if (clear_line)
				line += new String(' ', Console.WindowWidth - (line.Length + Console.CursorLeft)) + end;
			else
				line += end;

			Console.Write(line);
		}

		/// <summary>
		/// Append all logs to the file
		/// </summary>
		public static void WriteLog()
		{
			// write the logs to the file
			using (StreamWriter writer = new StreamWriter(filename, false, Encoding.UTF8))
			{
				writer.Write(sb.ToString());
			}

			// clear the buffer
			sb.Clear();
		}
	}
}
