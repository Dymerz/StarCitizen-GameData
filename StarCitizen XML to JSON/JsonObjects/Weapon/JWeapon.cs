using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace StarCitizen_XML_to_JSON.JsonObjects.Weapon
{
	internal class JWeapon : JObject
	{
		internal override string directory_name { get => "Weapons"; }

		public JWeapon(XmlDocument doc, FileInfo file, string destination, string source) : base(doc, file, destination, source) { }

		public override void Process()
		{
			var root = doc.SelectSingleNode("/*"); // get the root element
			var name = root.Name.Split('.')[1];

			var tags = LoadTags(root);
			var ammoContainer = LoadMagazine(root);
			var ammoParams = LoadAmmoParams(ammoContainer);

			if (ammoContainer != null)
			{
				if (ammoParams != null)
				{
					var ammoParams_imported = ammoContainer.ImportNode(ammoParams, true);
					ammoContainer.SelectSingleNode("*/Components/SAmmoContainerComponentParams").AppendChild(ammoParams_imported);
				}

				var ammo_imported = doc.ImportNode(ammoContainer.SelectSingleNode("/*"), true);
				doc.SelectSingleNode("*/Components/SCItemWeaponComponentParams").AppendChild(ammo_imported);
			}

			base.WriteFile(doc, name);
			base.ValidateFiles();
		}

		private string[] LoadTags(XmlNode root)
		{
			List<string> tags = new List<string>();

			foreach (XmlNode node in root.SelectNodes("./tags/*"))
			{
				string uuid = node.Attributes["value"]?.Value ?? null;
				if(uuid != null)
					tags.Add(CryXML.game.FindRef(uuid)?.Attributes["tagName"]?.Value);
			}

			return tags.ToArray();
		}

		private XmlDocument LoadMagazine(XmlNode root)
		{
			if (root == null)
				return null;

			var uuid = root.
				SelectSingleNode("./Components/SCItemWeaponComponentParams")?
				.Attributes["ammoContainerRecord"]?
				.Value;

			var ammoPath = CryXML.game.FindRef(uuid)?.Attributes["__path"].Value;

			if(ammoPath == null)
				return null;

			XmlDocument magazine = new XmlDocument();
			magazine.Load(Path.Combine(base.source, ammoPath));
			return magazine;
		}
		private XmlNode LoadAmmoParams(XmlNode root)
		{
			if (root == null)
				return null;

			var uuid = root.
				SelectSingleNode("*/Components/SAmmoContainerComponentParams").
				Attributes["ammoParamsRecord"]?.Value;

			if (uuid == null)
				return null;
			return CryXML.game.FindRef(uuid);
		}
	}
}
