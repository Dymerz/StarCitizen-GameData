using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace StarCitizen_XML_to_JSON
{
	public class CryGame
	{
		private string filename = "Game.xml";
		private XmlDocument doc;

		public CryGame(string path)
		{
			doc = new XmlDocument();
			doc.Load(Path.Combine(path, filename));
		}

		public XmlNode FindRef(string uuid)
		{
			return doc.SelectSingleNode($"//*[@__ref=\"{uuid}\"]");
		}

		public XmlNodeList FindRefs(string uuid)
		{
			return doc.SelectNodes($"//*[@__ref=\"{uuid}\"]");
		}
	}
}
