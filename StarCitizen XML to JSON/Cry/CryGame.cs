using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;

namespace StarCitizen_XML_to_JSON.Cry
{
	public class CryGame
	{
		private string game_filename = "Game.xml";
		private string product_prices_filename = @".\Libs\Subsumption\Shops\RetailProductPrices.xml";

		private string cache_filename = "Data.cache";

		private XmlDocument docGame;
		private XmlDocument docProductPrices;
		private List<CryCacheData> cache = new List<CryCacheData>();

		public CryGame(string path)
		{
			docProductPrices = new XmlDocument();
			docProductPrices.Load(Path.Combine(path, product_prices_filename));

			docGame = new XmlDocument();
			docGame.Load(Path.Combine(path, game_filename));

		}

		public XmlNode FindGameRef(string uuid)
		{
			if (uuid == "00000000-0000-0000-0000-000000000000")
				return null;

			// Search in Cache
			var res = GetCache("game_" + uuid);
			if (res != null)
				return res;

			// Search in Game.xml
			res = docGame.SelectSingleNode($"//*[@__ref=\"{uuid}\"]");
			AddCache("game_" + uuid, res);
			return res;
		}

		public XmlNode[] FindGameRefs(string uuid)
		{
			if (uuid == "00000000-0000-0000-0000-000000000000")
				return null;
			
			// Search in Cache
			XmlNode[] res = GetMultipleCache("game_" + uuid);
			if (res != null)
				return res;

			// Search in Game.xml
			var nodes = docGame.SelectNodes($"//*[@__ref=\"{uuid}\"]");
			AddMultipleCache("game_"+uuid, nodes);
			if (nodes == null)
				return null;

			// Convert XMlNodeList to XMlNode[]
			List<XmlNode> array_nodes = new List<XmlNode>();
			foreach (XmlNode n in nodes)
				array_nodes.Add(n);
			return array_nodes.ToArray();
		}

		public XmlNode FindPriceRef(string uuid)
		{
			if (uuid == "00000000-0000-0000-0000-000000000000")
				return null;

			// Search in Cache
			var res = GetCache("product_price_" + uuid);
			if (res != null)
				return res;

			// Search in RetailProductPrices.xml
			res = docProductPrices.SelectSingleNode($"//*[@ID=\"{uuid}\"]");
			AddCache("product_price_" + uuid, res);
			return res;
		}


		public bool LoadCache()
		{
			var path = Path.Combine(Program.assembly_directory, cache_filename);

			if (!File.Exists(path))
				return false;

			using (Stream stream = File.OpenRead(path))
			{
				BinaryFormatter bf = new BinaryFormatter();
				cache = (List<CryCacheData>)bf.Deserialize(stream);
			}
			return true;
		}

		public void SaveCache()
		{
			var path = Path.Combine(Program.assembly_directory, cache_filename);

			using (Stream stream = File.OpenWrite(path))
			{
				BinaryFormatter bf = new BinaryFormatter();
				bf.Serialize(stream, cache);
			}
		}

		public void DeleteCache()
		{
			var path = Path.Combine(Program.assembly_directory, cache_filename);

			if (!File.Exists(path))
				return;

			File.Delete(path);
		}

		private void AddCache(string uuid, XmlNode node)
		{
			if (!CacheExist(uuid) && node != null)
				cache.Add(new CryCacheData(uuid, node));
		}

		private void AddMultipleCache(string uuid, XmlNodeList nodes)
		{
			if (!CacheExist(uuid) && nodes != null)
			{
				var enu = nodes.GetEnumerator();
				while (enu.MoveNext())
				{
					cache.Add(new CryCacheData(uuid, (XmlNode)enu.Current));
				}
			}
		}

		private XmlNode GetCache(string uuid)
		{
			return cache.Find(c => c.uuid.Equals(uuid))?.node;
		}

		private XmlNode[] GetMultipleCache(string uuid)
		{
			return cache.FindAll(c => c.uuid.Equals(uuid)).ConvertAll(t => t.node)?.ToArray();
		}

		private bool CacheExist(string uuid)
		{
			return cache.Exists(c => c.node.Equals(uuid));
		}
	}
}
