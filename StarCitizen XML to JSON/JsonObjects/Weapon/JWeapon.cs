using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace StarCitizen_XML_to_JSON.JsonObjects.Weapon
{
	internal class JWeapon : JObject
	{
		internal override string directory_name { get => "Weapons"; }

		public JWeapon(XmlDocument doc, FileInfo file, string destination) : base(doc, file, destination) { }

		public override void Process()
		{
			var root = doc.SelectSingleNode("/*"); // get the root element

			base.WriteFile(doc, "Sample");

			base.ValidateFiles();
		}
	}
}
