using StarCitizen_XML_to_JSON.Cry;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace StarCitizen_XML_to_JSON.JsonObjects.JCommodity
{
	internal class JCommodity : JObject
	{
		internal override string directory_name { get => "Commodities"; }

		public JCommodity(XmlDocument doc, FileInfo file, string destination, string source) : base(doc, file, destination, source) { }

		public override void Process()
		{
			var root = doc.SelectSingleNode("/*"); // get the root element

			var tags = LoadTags(root);
			if (tags.Length == 0)
				throw new System.IndexOutOfRangeException("tags was length of 0");

			var name = tags?[0];
			if (name == null)
				throw new System.ArgumentNullException(nameof(name), "The name was null");

			var type = LoadType(root);
			if (type != null)
			{
				var type_imported = doc.ImportNode(type, true);
				doc.SelectSingleNode("*/Components/CommodityComponentParams").AppendChild(type_imported);
			}

			var subtype = LoadSubtype(root);
			if (subtype != null)
			{ 
				var subtype_imported = doc.ImportNode(subtype, true);
				doc.SelectSingleNode("*/Components/CommodityComponentParams").AppendChild(subtype_imported);
			}

			base.WriteFile(doc, name); // write the main ship

			base.ValidateFiles();
		}

		private string[] LoadTags(XmlNode root)
		{
			List<string> tags = new List<string>();

			foreach (XmlNode node in root.SelectNodes("./tags/*"))
			{
				string uuid = node.Attributes["value"]?.Value ?? null;
				if (uuid != null)
					tags.Add(CryXML.game.FindRef(uuid)?.Attributes["tagName"]?.Value);
			}

			return tags.ToArray();
		}

		private XmlNode LoadType(XmlNode root)
		{
			var uuid = root.SelectSingleNode("./Components/CommodityComponentParams").Attributes["type"].Value;
			return CryXML.game.FindRef(uuid);
		}

		private XmlNode LoadSubtype(XmlNode root)
		{
			var uuid = root.SelectSingleNode("./Components/CommodityComponentParams").Attributes["subtype"].Value;
			return CryXML.game.FindRef(uuid);
		}
	}
}
