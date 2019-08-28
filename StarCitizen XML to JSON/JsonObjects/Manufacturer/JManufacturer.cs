using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace StarCitizen_XML_to_JSON.JsonObjects.Manufacturer
{
	class JManufacturer : JObject
	{
		internal override string directory_name { get => "Manufacturers"; }

		public JManufacturer(XmlDocument doc, FileInfo file, string destination, string source) : base(doc, file, destination, source) { }

		/// <summary>
		/// Process the XML conversion
		/// </summary>
		public override void Process()
		{
			var root = doc.SelectSingleNode("/*"); // get the root element

			var name = root.Name.Split(".")[1];
			base.WriteFile(doc, name); // write the main ship

			base.ValidateFiles();
		}
	}
}

