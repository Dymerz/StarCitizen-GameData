using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace StarCitizen_XML_to_JSON.JsonObjects
{
	class JEntitySample : JObject
	{
		// The name of the directory
		internal override string directory_name { get => "JEntitySample"; }

		// [DO NOT CHANGE] Default constructor
		public JEntitySample(FileInfo file, string destination, string source) : base(file, destination, source) { }

		/// <summary>
		/// Process the XML conversion
		/// </summary>
		public override void Process()
		{ 
			// Get the root element
			var root = doc.SelectSingleNode("/*"); 

			// The name of the file
			var name = "JEntitySample";

			// Write the document to the disk
			base.WriteFile(doc, name);
		}
	}
}

