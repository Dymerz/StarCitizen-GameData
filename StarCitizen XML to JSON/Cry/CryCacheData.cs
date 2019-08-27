using System;
using System.Runtime.Serialization;
using System.Xml;

namespace StarCitizen_XML_to_JSON.Cry
{
	[Serializable]
	class CryCacheData : ISerializable
	{
		public string uuid;
		public XmlNode node;

		public CryCacheData(string uuid, XmlNode node)
		{
			this.uuid = uuid ?? throw new ArgumentNullException(nameof(uuid));
			this.node = node ?? throw new ArgumentNullException(nameof(node));
		}

		// Deserialize
		public CryCacheData(SerializationInfo info, StreamingContext context)
		{
			uuid = (string)info.GetValue("uuid", typeof(string));

			XmlDocument doc = new XmlDocument();
			doc.LoadXml((string)info.GetValue("node", typeof(string)));
			node = doc.SelectSingleNode("/*");
		}

		// Serialize
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("uuid", uuid);
			info.AddValue("node", node.OuterXml);
		}
	}
}
