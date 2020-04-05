using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StarCitizen_XML_to_JSON.Cry
{
	public enum Language {
		ENGLISH = 0,
		FRENCH = 1,
		GERMAN = 2,
	}

	public class CryLocalization
	{

		private string path = @"./Localization/{0}/global.ini";
		private string source;
		private List<Tuple<string, string>> localizations = new List<Tuple<string, string>>();

		public CryLocalization(string source)
		{
			this.source = source;
		}

		public void LoadLocalization(Language language)
		{
			// Load all key=value in dict
			localizations.Clear();
			
			using (StreamReader reader = new StreamReader(Path.Combine(source, String.Format(path, language))))
			{
				while (reader.Peek() != -1)
				{
					string line = reader.ReadLine();
					string[] args = line.Split('=');

					string key = args[0];
					string value = args[1];

					// Sanatize value
					value = value.Replace("\"", "\\\"");
					value = value.Replace("'", "\\'");

					localizations.Add(new Tuple<string, string>("@"+key, value));
				}
			}
		}

		public string FindKey(string key)
		{
			// search key and return value

			return localizations.Where((l) =>
				l.Item1.ToLower() == key.ToLower()
			).FirstOrDefault((l) => l.Item2 != null)?.Item2;
		}
	} 
}
