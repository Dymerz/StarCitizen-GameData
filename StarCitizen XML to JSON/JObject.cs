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
		public string filename;
		public string source;
		public string destination;

		public string directory_name = null;

		protected JObject(XmlDocument doc, FileInfo file, string destination)
		{
			this.doc = doc ?? throw new ArgumentNullException(nameof(doc));
			this.file = file ?? throw new ArgumentNullException(nameof(file));
			this.destination = destination ?? throw new ArgumentNullException(nameof(destination));

			doc.Load(file.FullName);
		}

		public abstract void Process();

		public string Sanitize(string value)
		{
			value = value.Replace("_", " ");

			return value;
		}
	}
}
