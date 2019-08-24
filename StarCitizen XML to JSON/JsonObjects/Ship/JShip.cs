using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Newtonsoft.Json;

namespace StarCitizen_XML_to_JSON.JsonObjects.Ship
{
	internal class JShip : JObject
	{
		public new string directory_name = "Ships";

		public JShip(XmlDocument doc, FileInfo file, string destination) : base(doc, file, destination)
		{
			if (!Directory.Exists(Path.Combine(destination, directory_name)))
				Directory.CreateDirectory(Path.Combine(destination, directory_name));
		}

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

			WriteFile(doc); // write the main ship

			// write all models of the main ship
			foreach (var m in models)
			{
				// remove the <Modifications> tag
				modif = m.model.FirstChild.SelectSingleNode("//Modifications");
				if (modif != null) modif.ParentNode.RemoveChild(modif);

				if (m.model != null)
					WriteFile(m.model);
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

		/// <summary>
		/// Write to a JSON file the edited XMLDocument
		/// </summary>
		/// <param name="doc">document to write</param>
		private void WriteFile(XmlDocument doc)
		{
			XmlNode root = doc.FirstChild;

			var name = (root.Attributes["displayname"] ?? root.Attributes["name"]).Value;
			var filename = Path.Combine(
					base.destination, directory_name, name.Replace(" ", "_") + ".json");

			using (StreamWriter writer = new StreamWriter(filename))
			{
				// serealize XML to JSON
				var plain_json = JsonConvert.SerializeXmlNode(root, Newtonsoft.Json.Formatting.Indented, true);

				// remove @ before property name
				plain_json = Regex.Replace(plain_json, "([\"\'])(@)((.*?)[\"\']\\:)", "$1$3");

				// remove bad leading zero (eg: '01.5' or '040')
				plain_json = Regex.Replace(plain_json, "([\"\'])(0)([0-9\\.]+)([\"\'])", "$1$3$4");

				// add leading 0 on decaml ".x"
				plain_json = Regex.Replace(plain_json, "([\"\'])(\\.[0-9]+)([\"\'])", "0$2");

				// remove '"' to numbers (int, float, double,..)
				plain_json = Regex.Replace(plain_json, "([\"\'])([0-9]*[(\\.[0-9]+]?)([\"\'])", "$2");

				writer.Write(plain_json);
			}

			// add the file to the files list to be validated later
			base.generatedFiles.Add(new FileInfo(filename));
		}
	}
}
