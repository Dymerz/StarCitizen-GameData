using System;
using System.Runtime.Serialization;
using System.Text;
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
			string encoded_uuid = (string)info.GetValue("uuid", typeof(string));
			uuid = Encoding.UTF8.GetString(Convert.FromBase64String(encoded_uuid));

			XmlDocument doc = new XmlDocument();
			string encoded_xml = (string)info.GetValue("node", typeof(string));
			doc.LoadXml(Encoding.UTF8.GetString(Convert.FromBase64String(encoded_xml)));
			node = doc.SelectSingleNode("/*");
		}

		// Serialize
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("uuid", Convert.ToBase64String(Encoding.UTF8.GetBytes(uuid)));
			info.AddValue("node", Convert.ToBase64String(Encoding.UTF8.GetBytes(node.OuterXml)));
		}
	}
}
