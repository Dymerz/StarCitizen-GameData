using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StarCitizen_XML_to_JSON
{
	class ProgressBar : IEnumerable
	{
		private IList list;

		private int width = 0;

		private int length = 0;
		private int count = 0;
		private int leading_space = 11;

		private int title_max = 0;
		private string title;

		private bool auto_title = false;
		private bool clear_line = false;

		/// <summary>
		/// Create a loading bar at the bottom of the terminal
		/// </summary>
		/// <param name="list">the object to iterate from.</param>
		/// <param name="title">A friendly name to print.</param>
		/// <param name="clear_line">if true, this will clear the line when the process is done.</param>
		public ProgressBar(IList list, string title = "Progress", bool clear_line = false, bool auto_title = false)
		{
			this.list = list ?? throw new ArgumentNullException(nameof(ProgressBar.list));
			this.length = list.Count;
			this.width = Console.WindowWidth;

			this.title = title;
			this.auto_title = auto_title;
			this.clear_line = clear_line;

			// if auto_title is true
			if (auto_title)
				// get the longest titles object
				this.title_max = list.Cast<object>().Max(l => l.ToString().Length);
			else
				// else, get the length of the title
				this.title_max = title.Length;

			Refresh();
		}

		/// <summary>
		/// Called by the foreach
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			var en = list.GetEnumerator(); // get the enumerator of the object

			// iterate each object
			while (en.MoveNext())
			{
				if (auto_title)
					title = en.Current.ToString();

				yield return en.Current; // return the object to the foreach loop

				// then clean the line with spaces
				Console.Write('\r' + new String(' ', width - 1) + '\r');

				// increment the number of objects already processed
				count++;

				// Refresh the progress bar
				Refresh();

			}

			if (clear_line)
			{
				int cursor_posLeft = Console.CursorLeft;
				int cursor_posTop = Console.CursorTop;

				Console.CursorTop = Console.WindowTop + Console.WindowHeight - 1;
				Console.Write('\r' + new String(' ', width - 1) + '\r');

				Console.SetCursorPosition(cursor_posLeft, cursor_posTop);
			}
			else
				Console.Write("\r\n");
		}

		/// <summary>
		/// Refresh the progress
		/// </summary>
		private void Refresh()
		{
			int cursor_posLeft = Console.CursorLeft; // get the cursor position (left)
			int cursor_posTop = Console.CursorTop; // get the cursor position (right)

			int percent = (int)((float)count / length * 100); // calculate the progress percentage

			int progress_space = (width - 2) - title_max - leading_space; // calculate the area of the progress
			int val = (int)(progress_space * ((float)percent / 100)); // calculate the number of '#' to print

			string progress = new String('#', val); // create the progress (###) string
			string remaining = new String('.', progress_space - progress.Length); // create the empty progress string

			// Scrolling
			// if the cursor is on the progress bar, then place the cursor one line below
			//	(it will print the progress one line bellow)
			if (Console.WindowTop + Console.WindowHeight - 1 == Console.CursorTop)
				Console.CursorTop++;

			// place the cursor at the last line
			Console.CursorTop = Console.WindowTop + Console.WindowHeight - 1;

			// print all progress info (e.g: "Progress: [ 50%] ")
			Console.Write(String.Format("{0," + title_max + "}: [{1,3:##0}%] ", title, percent));

			// print the loading progress (e.g: ##....)
			Console.Write($"[{progress}{remaining}]");

			// place the cursor back to his original position
			Console.SetCursorPosition(cursor_posLeft, cursor_posTop);
		}
	}
}
