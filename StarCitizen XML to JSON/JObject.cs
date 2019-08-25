using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace StarCitizen_XML_to_JSON
{
	abstract class JObject
	{
		public XmlDocument doc { get; private protected set; } = null;
		public FileInfo file;
		internal List<FileInfo> generatedFiles;

		public string destination;
		public string source;

		abstract internal string directory_name { get;}

	
		protected JObject(XmlDocument doc, FileInfo file, string destination, string source)
		{
			this.doc = doc ?? throw new ArgumentNullException(nameof(doc));
			this.file = file ?? throw new ArgumentNullException(nameof(file));
			this.destination = destination ?? throw new ArgumentNullException(nameof(destination));
			this.source = source ?? throw new ArgumentNullException(nameof(source));

			this.generatedFiles = new List<FileInfo>();
			doc.Load(file.FullName);

			if (!Directory.Exists(Path.Combine(destination, directory_name)))
				Directory.CreateDirectory(Path.Combine(destination, directory_name));
		}

		public abstract void Process();
		/// <summary>
		/// Valid all generated files by loading them
		/// </summary>
		public void ValidateFiles()
		{
				if (generatedFiles.Count == 0)
					throw new Exception("The variable 'generatedFiles' can't empty");

				foreach (var file in generatedFiles)
				{
					using (StreamReader reader = new StreamReader(file.FullName))
					{
						JsonConvert.DeserializeObject(reader.ReadToEnd());
					}
				}
		}

		/// <summary>
		/// Write to a JSON file the edited XMLDocument
		/// </summary>
		/// <param name="doc">document to write</param>
		public void WriteFile(XmlDocument doc, string name)
		{
			XmlNode root = doc.FirstChild;

			var filename = Path.Combine(
					destination, directory_name, name.Replace(" ", "_") + ".json");

			using (StreamWriter writer = new StreamWriter(filename))
			{
				// serealize XML to JSON
				var plain_json = JsonConvert.SerializeXmlNode(root, Newtonsoft.Json.Formatting.Indented, true);

				// remove @ before property name
				plain_json = Regex.Replace(plain_json, "([\"\'])(@)((.*?)[\"\']\\:)", "$1$3");

				// remove bad leading zero (eg: '01.5' or '040')
				plain_json = Regex.Replace(plain_json, "([\"\'])(0)([0-9\\.]+)([\"\'])", "$1$3$4");

				// add leading 0 on decaml ".x"
				plain_json = Regex.Replace(plain_json, "([\"\'])(\\.[0-9]+)([\"\'])", "0$2");

				// remove '"' to numbers (int, float, double,..)
				plain_json = Regex.Replace(plain_json, "([\"\'])([0-9]*[(\\.[0-9]+]?)([\"\'])", "$2");

				writer.Write(plain_json);
			}

			// add the file to the files list to be validated later
			generatedFiles.Add(new FileInfo(filename));
		}

		public string Sanitize(string value)
		{
			value = value.Replace("_", " ");

			return value;
		}
	}
}
