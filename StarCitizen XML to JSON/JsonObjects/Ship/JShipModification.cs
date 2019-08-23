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
					var id = node.Attributes["id"].Value;	// get the id
					var target = doc.SelectSingleNode($"//*[@id='{id}']");   // get the target of the id
					var target_parent = target.ParentNode;

					XmlNode xmlNode = doc.CreateNode(node.NodeType, node.Name, node.NamespaceURI);
					XmlNode importedNode = doc.ImportNode(xmlNode, true);
					//target.AppendChild(importedNode);

					target.InnerXml = node.InnerXml;
					//target.ParentNode.ReplaceChild(importedNode, target);
				}
			}

			// return results as new XMLDocument
			return doc;
		}

		/*
		private XmlDocument RecursiveModification(XmlNode source, XmlDocument doc, XmlNode target = null)
		{
			foreach (XmlAttribute a in source.Attributes)
			{
				if (source.Attributes["id"] != null)
				{
					var id = source.Attributes["id"].Value;
					target = doc.SelectSingleNode($"//*[@id='{id}']");
				}
				if (target.Attributes[a.Name] == null)
				{
					var n = doc.CreateAttribute(a.Name);
					n.Value = a.Value;
					target.Attributes.SetNamedItem(n);
				}else
					target.Attributes[a.Name].Value = a.Value;
			}

			if (source.HasChildNodes)
			{
				target = target.FirstChild;
				foreach (XmlNode n in source.ChildNodes)
				{
					if (target != null)
					{
						target.Attributes.RemoveAll();
						doc = RecursiveModification(n, doc, target);

					}

					if (target.NextSibling != null)
						target = target.NextSibling;
					else
					{
						XmlNode xmlNode = doc.CreateNode(n.NodeType, n.Name, n.NamespaceURI);
						XmlNode importedNode = doc.ImportNode(xmlNode, true);
						target.AppendChild(importedNode);
					}
				}
			}

			if (source.NextSibling != null)
			{
				source = source.NextSibling;
				if (target.NextSibling != null)
					doc = RecursiveModification(source, doc, target.NextSibling);
				else
				{
					XmlNode xmlNode = doc.CreateNode(source.NodeType, source.Name, source.NamespaceURI);
					XmlNode importedNode = doc.ImportNode(xmlNode, true);
					target.AppendChild(importedNode);

					if (source.NextSibling != null)
					{
						doc = RecursiveModification(source.NextSibling, doc, target.NextSibling);
					}
				}
			}

			doc = target.OwnerDocument;
			return doc;
		}
		*/
	}
}
