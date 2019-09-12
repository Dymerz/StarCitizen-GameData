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
		public XmlDocument model { get; private set; } = null;

		public JShipModification(string name, string patchFile, XmlNode source_node)
		{
			this.name = name ?? throw new ArgumentNullException(nameof(name));
			this.patchFile = patchFile;

			model = new XmlDocument();
			model.LoadXml(source_node.OwnerDocument.InnerXml);

			model = getElements(source_node, model);

			if (patchFile != null && System.IO.File.Exists(patchFile))
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(patchFile);
				model = getElements(doc, model);
			}
		}
		public JShipModification(string name, XmlNode node) : this(name, null, node) { }

		public XmlDocument getElements(XmlNode root, XmlDocument doc)
		{
			if (doc == null)
				doc = root.OwnerDocument ?? (XmlDocument)root;
			
			// check if has child(ren)
			if (!root.HasChildNodes)
				return doc;

			// replace each modifications in the new XMLDocument
			foreach (XmlNode node in root.SelectSingleNode("./*").ChildNodes)
			{

				// if it's an <Elem>, replace values of each Elem
				if (node.Name == "Elem")
				{
					var id = node.Attributes["idRef"]?.Value ?? node.Attributes["id"]?.Value; // get the idRef (if null -> id )
					var target = doc.SelectSingleNode($"//*[@id='{id}']");	// get the target of the id

					// if has target
					if (target != null)
					{
						// if attribute doesn't exist in the target, create one
						if (target.Attributes[node.Attributes["name"].Value] == null)
						{
							var n_attr = doc.CreateAttribute(node.Attributes["name"].Value);
							n_attr.Value = node.Attributes["value"].Value;
							target.Attributes.SetNamedItem(n_attr);
						}
						else
							// replace value of the attribute
							target.Attributes[node.Attributes["name"].Value].Value = node.Attributes["value"].Value;
					}
				}// if it's a modification file
				else
				{

					foreach (XmlNode child in node.SelectSingleNode("/").ChildNodes)
					{
						var id = node.Attributes["id"].Value;   // get the id
						var target = doc.SelectSingleNode($"//*[@id='{id}']");   // get the target of the id

						target = RecursiveReplace(target, node);
					}
				}
			}

			// return results as new XMLDocument
			return doc;
		}

		private XmlNode RecursiveReplace(XmlNode source, XmlNode editions)
		{
			foreach (XmlAttribute attr in editions.Attributes)
			{
				if (source.Attributes[attr.Name] != null)
					source.Attributes[attr.Name].Value = attr.Value;
				else
				{
					var imported = source.OwnerDocument.CreateAttribute(attr.Prefix, attr.LocalName, attr.NamespaceURI);
					imported.Value = attr.Value;
					source.Attributes.Append(imported);
				}
			}

			if (editions.HasChildNodes)
			{
				var nextEdition = editions.FirstChild;
				var nextSource = source.SelectSingleNode($"//{nextEdition.Name}");

				if (nextSource != null)
				{
					var imported = source.OwnerDocument.ImportNode(nextEdition, true);
					source.AppendChild(imported);
				}
				else
					nextSource = RecursiveReplace(nextSource, nextEdition);
			}

			var next = editions.NextSibling;
			if (next != null)
			{
				var nextSource = source.SelectSingleNode($"//{next.Name}");
				nextSource = RecursiveReplace(nextSource, next);
			}
			return source;
		}
	}
}
