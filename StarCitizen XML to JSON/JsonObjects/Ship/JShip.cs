using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

		public override void Process()
		{
			var root = doc.SelectSingleNode("/*");

			var models = GetShipsModels(root); // Get all ships models XML
			foreach (var m in models)
			{
				if(m.model != null)
					WriteFile(m.model);
			}

		}

		private JShipModification[] GetShipsModels(XmlNode root)
		{
			List<JShipModification> modifications = new List<JShipModification>();

			// check if has modifications
			if (root.SelectSingleNode("/Vehicle/Modifications") == null)
				return new JShipModification[0];

			// Load all modifications
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

		private void WriteFile(XmlDocument doc)
		{
			XmlNode root = doc.FirstChild;

			var name = (root.Attributes["displayname"] ?? root.Attributes["name"]).Value;

			StringBuilder sb = new StringBuilder();
			StringWriter sw = new StringWriter(sb);
			using (JsonWriter writer = new JsonTextWriter(sw))
			{
				writer.WriteStartObject();

				writer.WritePropertyName("displayname");

				writer.WriteValue(Sanitize(name));


				writer.WriteEndObject();
			}

			using (StreamWriter writer = new StreamWriter(Path.Combine(base.destination, directory_name, name.Replace(" ", "_") + ".json")))
			{
				writer.Write(sb.ToString());
			}
		}
	}
}
