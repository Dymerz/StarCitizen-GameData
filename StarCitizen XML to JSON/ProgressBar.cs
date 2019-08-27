using System;
using System.Collections;

namespace StarCitizen_XML_to_JSON
{
	class ProgressBar : IEnumerable
	{
		private IList list;

		private int width = 0;
		private int height = 0;

		private int length = 0;
		private int count = 0;
		private int leading_space = 11;

		private string title;
		private bool clear_line = false;

		public ProgressBar(IList list, string title = "Progress", bool clear_line = false)
		{
			this.list = list ?? throw new ArgumentNullException(nameof(ProgressBar.list));
			this.length = list.Count;
			this.width = Console.WindowWidth;
			this.height = Console.WindowHeight;

			this.title = title;
			this.clear_line = clear_line;

			Refresh();
		}

		public IEnumerator GetEnumerator()
		{
			var en = list.GetEnumerator();
			while (en.MoveNext())
			{
				yield return en.Current;

				Console.Write('\r' + new String(' ', width) + '\r');
				count++;
				Refresh();
			}

			if (clear_line)
			{
				int cursor_posLeft = Console.CursorLeft;
				int cursor_posTop = Console.CursorTop;

				Console.CursorTop = Console.WindowTop + Console.WindowHeight - 1;
				Console.Write('\r' + new String(' ', width) + '\r');

				Console.SetCursorPosition(cursor_posLeft, cursor_posTop);
			}else
				Console.Write("\r\n");
		}

		private void Refresh()
		{
			int cursor_posLeft = Console.CursorLeft;
			int cursor_posTop = Console.CursorTop;

			int percent = (int)((float)count / length * 100);

			int progress_space = (width - 2) - title.Length - leading_space;
			int val = (int)(progress_space * ((float)percent / 100));
			string progress = new String('#', val);
			string remaining = new String('.', progress_space - progress.Length);

			if (Console.WindowTop + Console.WindowHeight - 1 == Console.CursorTop)
				Console.CursorTop++;

			// place the cursor at the last line
			Console.CursorTop = Console.WindowTop + Console.WindowHeight - 1;

			Console.Write($"{title}: [{String.Format("{0,3:##0}", percent)}%] [");
			Console.Write(progress + remaining);
			Console.Write("]");

			Console.SetCursorPosition(cursor_posLeft, cursor_posTop);
		}
	}
}
