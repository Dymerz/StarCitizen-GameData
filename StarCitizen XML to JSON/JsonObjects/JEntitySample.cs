using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace StarCitizen_XML_to_JSON.JsonObjects
{
	class JEntitySample : JObject
	{
		//public new string directory_name = "Ships";
		internal override string directory_name { get => "JEntitySample"; }

		public JEntitySample(XmlDocument doc, FileInfo file, string destination) : base(doc, file, destination) { }

		/// <summary>
		/// Process the XML conversion
		/// </summary>
		public override void Process()
		{
			var root = doc.SelectSingleNode("/*"); // get the root element

			var name = "JEntitySample";
			base.WriteFile(doc, name); // write the main ship

			base.ValidateFiles();
		}
	}
}

