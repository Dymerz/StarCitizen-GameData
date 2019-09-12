using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;
using System.Xml.Linq;

namespace StarCitizen_XML_to_JSON.JsonObjects.Ship
{
	internal class JShip : JObject
	{
		internal override string directory_name { get => "Ships"; }

		public JShip(XmlDocument doc, FileInfo file, string destination, string source) : base(doc, file, destination, source) { }

		/// <summary>
		/// Process the XML conversion
		/// </summary>
		public override void Process()
		{
			var root = doc.SelectSingleNode("/*"); // get the root element
			var models = GetShipsModels(root); // get all ships models XML

			// remove the <Modifications> tag
			var modif = doc.SelectSingleNode("//Modifications");
			if (modif != null) modif.ParentNode.RemoveChild(modif);


			var root_name = (root.Attributes["displayname"] ?? root.Attributes["name"]).Value;
			base.WriteFile(doc, root_name); // write the main ship

			// write all models of the main ship
			foreach (var m in models)
			{
				// remove the <Modifications> tag
				modif = m.model.FirstChild.SelectSingleNode("//Modifications");
				if (modif != null) modif.ParentNode.RemoveChild(modif);

				if (m.model != null)
				{
					var name = (m.model.SelectSingleNode("/*").Attributes["displayname"] ?? m.model.SelectSingleNode("/*").Attributes["name"]).Value;

					// TODO: some models erase an existing model
					// Check with AEGS AEGS_Avenger (Titan)
					base.WriteFile(m.model, name);
				}	
			}

			base.ValidateFiles();
		}

		/// <summary>
		/// Parse from XML all modifications (models)
		/// </summary>
		/// <param name="root"></param>
		/// <returns></returns>
		private JShipModification[] GetShipsModels(XmlNode root)
		{
			List<JShipModification> modifications = new List<JShipModification>();

			// check if has modifications
			if (root.SelectSingleNode("/Vehicle/Modifications") == null)
				return new JShipModification[0];

			// load all modifications
			foreach (XmlNode node in root.SelectSingleNode("/Vehicle/Modifications").ChildNodes)
			{
				var n_name = node.Attributes["name"]?.Value ?? null;
				var n_patchFile = node.Attributes["patchFile"]?.Value ?? null;
				if (n_patchFile != null)
					n_patchFile = Path.Combine(file.DirectoryName, n_patchFile + ".xml");

				if (n_name == null)
					continue;
				// start replacement of modification
				modifications.Add(new JShipModification(n_name, n_patchFile, node));
			}

			return modifications.ToArray();
		}
	}
}
