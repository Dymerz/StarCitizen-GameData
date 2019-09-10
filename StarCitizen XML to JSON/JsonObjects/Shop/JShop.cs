using StarCitizen_XML_to_JSON.Cry;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace StarCitizen_XML_to_JSON.JsonObjects.JShop
{
	class JShop : JObject
	{
		internal override string directory_name { get => "Shops"; }

		public JShop(XmlDocument doc, FileInfo file, string destination, string source) : base(doc, file, destination, source) { }

		/// <summary>
		/// Process the XML conversion
		/// </summary>
		public override void Process()
		{
			var root = doc.SelectSingleNode("/*"); // get the root element

			foreach (XmlNode layout in root.SelectNodes("./ShopLayoutNodes/*"))
			{
				XmlDocument doc_layout = new XmlDocument();
				doc_layout.LoadXml(layout.OuterXml);

				foreach (XmlNode shopLayoutNode in layout.SelectNodes("./ShopLayoutNodes/ShopLayoutNode"))
				{
					foreach (XmlNode shopInventoryNode in shopLayoutNode.SelectNodes("./ShopInventoryNodes/ShopInventoryNode"))
					{
						var uuid = shopInventoryNode.Attributes["InventoryID"].Value.ToString();

						var prices = CryXML.game.FindPriceRef(uuid);
						if (prices != null)
						{
							var imported_price = doc_layout.ImportNode(prices, true);
							var node_price = doc_layout.CreateElement("Prices");
							node_price.InnerXml = imported_price.OuterXml;
							doc_layout.SelectSingleNode("/*").AppendChild(node_price);
						}
						
						var entity = CryXML.game.FindGameRef(uuid);
						if (entity != null)
						{
							var imported_entity = doc_layout.ImportNode(entity, true);
							var node_entity = doc_layout.CreateElement("Entity");
							node_entity.InnerXml = imported_entity.OuterXml;
							doc_layout.SelectSingleNode("/*").AppendChild(node_entity);
						}
					}
				}

				var name = (layout.Attributes["Name"]?.Value ?? layout.Attributes["name"]?.Value).ToString();
				base.WriteFile(doc_layout, name);
			}

			base.ValidateFiles();
		}
	}
}

