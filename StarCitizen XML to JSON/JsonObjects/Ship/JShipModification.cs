using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace StarCitizen_XML_to_JSON.JsonObjects.Ship
{
	class JShipModification
	{
		public string name { get; private set; }
		public string patchFile { get; private set; }
		public List<JShipModificationElement> elements { get; private set; }

		public JShipModification(string name, string patchFile, XmlNode source_node)
		{
			this.name = name ?? throw new ArgumentNullException(nameof(name));
			this.patchFile = patchFile;


			elements = new List<JShipModificationElement>();
			//elements.AddRange(getElements(source_node));

			if (patchFile != null && System.IO.File.Exists(patchFile))
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(patchFile);
				elements.AddRange(getElements(doc, source_node.OwnerDocument));
			}
		}
		public JShipModification(string name, XmlNode node) : this(name, null, node) { }

		public JShipModificationElement[] getElements(XmlNode root, XmlDocument doc = null)
		{
			if (doc == null)
				doc = root.OwnerDocument;
			
			List<JShipModificationElement> elements = new List<JShipModificationElement>();

			foreach (XmlNode node in root.SelectSingleNode("./*").ChildNodes)
			{
				if (node.Name == "Elem")
				{
					elements.Add(new JShipModificationElement(
						node.Attributes["idRef"].Value,
						node.Attributes["name"].Value,
						node.Attributes["value"].Value));
				}
				else
				{
					RecursiveModification(node.FirstChild, out doc);

					/*
					var id = node.Attributes["id"].Value;
					var source = node.FirstChild;
					var target = doc.SelectSingleNode($"//*[@id='{id}']/{node.FirstChild.Name}");

					Console.WriteLine(source.Attributes["engineWarmupDelay"].Value);
					Console.WriteLine(target.Attributes["engineWarmupDelay"].Value);

					foreach (XmlAttribute a in source.Attributes)
					{
						target.Attributes[a.Name].Value = a.Value;
					}
					Console.WriteLine(target);
					*/
				}
			}
			return elements.ToArray();
		}

		private void RecursiveModification(XmlNode source, out XmlDocument target)
		{
			throw new NotImplementedException();
		}
	}
}
