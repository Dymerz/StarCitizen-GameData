using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StarCitizen_XML_to_JSON
{
	class Progress
	{
		private string[] loading_bars = { "--", "\\ ", "| ", "/ ", "--", "\\ ", "| ", "/ " };

		private string done_message;
		private Task task;
		private int index;
		private bool run;

		private int animation_delay = 100;

		public Progress(string done_message = "")
		{
			this.index = 0;
			this.run = false;
			this.done_message = done_message;
		}

		public static object Process(Func<object> func, string done_messsage = "")
		{
			var l = new Progress(done_messsage);
			object result;

			l.Start();
			result = func();
			l.Stop();

			return result;
		}
		public static void Process(Action action, string done_messsage = "")
		{
			var l = new Progress(done_messsage);

			l.Start();
			action();
			l.Stop();
		}

		public void Start()
		{
			run = true;
			Console.Write("  ");

			task = Task.Run(() => 
			{
				while (task.Status == TaskStatus.Running && run)
				{
					Loop();
					task.Wait(animation_delay);
				}
			});
		}


		public void Stop()
		{
			run = false;
			while (task.Status == TaskStatus.Running)
				Thread.Sleep(1);

			task.Dispose();
			Console.Write("\b\b");
			Console.WriteLine(this.done_message);
		}

		private void Loop()
		{
			Console.Write("\b\b");
			Console.Write(loading_bars[index]);
			
			index = (++index % loading_bars.Length);
		}
	}
}
