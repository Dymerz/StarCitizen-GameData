using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace StarCitizen_XML_to_JSON
{
	abstract class JObject
	{
		public XmlDocument doc { get; private protected set; } = null;
		public FileInfo file;
		internal List<FileInfo> generatedFiles;

		public string destination;

		public string directory_name = null;

	
		protected JObject(XmlDocument doc, FileInfo file, string destination)
		{
			this.doc = doc ?? throw new ArgumentNullException(nameof(doc));
			this.file = file ?? throw new ArgumentNullException(nameof(file));
			this.destination = destination ?? throw new ArgumentNullException(nameof(destination));

			this.generatedFiles = new List<FileInfo>();
			doc.Load(file.FullName);
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
						Newtonsoft.Json.JsonConvert.DeserializeObject(reader.ReadToEnd());
					}
				}
		}

		public string Sanitize(string value)
		{
			value = value.Replace("_", " ");

			return value;
		}
	}
}
