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
		private List<Tuple<string, XmlNode>> cache = new List<Tuple<string, XmlNode>>();

		public CryGame(string path)
		{
			doc = new XmlDocument();
			doc.Load(Path.Combine(path, filename));
		}

		public XmlNode FindRef(string uuid)
		{
			if (uuid == "00000000-0000-0000-0000-000000000000")
				return null;

			// Search in Cache
			var res = GetCache(uuid);
			if (res != null)
				return res;

			// Search in Game.xml
			res = doc.SelectSingleNode($"//*[@__ref=\"{uuid}\"]");
			AddCache(uuid, res);
			return res;
		}

		public XmlNode[] FindRefs(string uuid)
		{
			if (uuid == "00000000-0000-0000-0000-000000000000")
				return null;
			
			// Search in Cache
			XmlNode[] res = GetMultipleCache(uuid);
			if (res != null)
				return res;

			// Search in Game.xml
			var nodes = doc.SelectNodes($"//*[@__ref=\"{uuid}\"]");
			AddMultipleCache(uuid, nodes);
			if (nodes == null)
				return null;

			// Convert XMlNodeList to XMlNode[]
			List<XmlNode> array_nodes = new List<XmlNode>();
			foreach (XmlNode n in nodes)
				array_nodes.Add(n);
			return array_nodes.ToArray();
		}
		private void AddCache(string uuid, XmlNode node)
		{
			if (!CacheExist(uuid) && node != null)
				cache.Add(new Tuple<string, XmlNode>(uuid, node));
		}

		private void AddMultipleCache(string uuid, XmlNodeList nodes)
		{
			if (!CacheExist(uuid) && nodes != null)
			{
				var enu = nodes.GetEnumerator();
				while (enu.MoveNext())
				{
					cache.Add(new Tuple<string, XmlNode>(uuid, (XmlNode)enu.Current));
				}
			}
		}

		private XmlNode GetCache(string uuid)
		{
			return cache.Find(c => c.Item1.Equals(uuid))?.Item2;
		}

		private XmlNode[] GetMultipleCache(string uuid)
		{
			return cache.FindAll(c => c.Item1.Equals(uuid)).ConvertAll(t => t.Item2)?.ToArray();
		}

		private bool CacheExist(string uuid)
		{
			return cache.Exists(c => c.Item1.Equals(uuid));
		}
	}
}
