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
		public new string directory_name = "Ship";

		public JShip(XmlDocument doc, FileInfo file, string destination) : base(doc, file, destination)
		{
			if (!Directory.Exists(Path.Combine(destination, directory_name)))
				Directory.CreateDirectory(Path.Combine(destination, directory_name));

			this.filename = Path.Combine(destination, directory_name, file.Name.Replace(file.Extension, null) + ".json");
		}

		public override void Process()
		{
			StringBuilder sb = new StringBuilder();
			StringWriter sw = new StringWriter(sb);
			using (JsonWriter writer = new JsonTextWriter(sw))
			{
				var root = doc.SelectSingleNode("/*");

				writer.WriteStartObject();

				writer.WritePropertyName("displayname");

				var name = Sanitize((root.Attributes["displayname"] ?? root.Attributes["name"]).Value);
				writer.WriteValue(name);

				List<JShipModification> modifications = new List<JShipModification>();
				foreach (XmlNode node in root.SelectSingleNode("/Vehicle/Modifications").ChildNodes)
				{
					var n_name = node.Attributes["name"]?.Value ?? null;
					var n_patchFile = node.Attributes["patchFile"]?.Value ?? null;
					if (n_patchFile != null)
						n_patchFile = Path.Combine(file.DirectoryName, n_patchFile + ".xml");

					modifications.Add(new JShipModification(n_name, n_patchFile , node));
				}

				writer.WriteEndObject();
			}


			using (StreamWriter writer = new StreamWriter(filename))
			{
				writer.Write(sb.ToString());
			}
			//Environment.Exit(0);
		}

		private void LoadModifications()
		{

		}
	}
}
