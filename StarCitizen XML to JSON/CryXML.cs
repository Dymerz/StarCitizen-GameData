using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using StarCitizen_XML_to_JSON.JsonObjects.Ship;

namespace StarCitizen_XML_to_JSON
{
	/// <summary>
	/// Convert all types of XML
	/// </summary>
	public class CryXML
	{
		public string source{ get; private set;  } = null;
		public string destination { get; private set;  } = null;

		public CryXML(string source, string destination)
		{
			this.source = source ?? throw new ArgumentNullException(nameof(source));
			this.destination = destination ?? throw new ArgumentNullException(nameof(destination));

			Directory.CreateDirectory(this.destination);
		}
		/// <summary>
		/// Convert a file into a JSON representation
		/// </summary>
		/// <param name="file">FileInfo: the file to convert</param>
		/// <param name="filter">SCType: filter</param>
		public void ConvertJSON(FileInfo file, SCType filter = SCType.Every)
		{
			// Get the relative and absolute file path

			// Load the XML
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(File.ReadAllText(file.FullName));

			// Get the file type
			SCType type = DetectType(doc);

			// Cast JObject to the right type
			JObject jObject = null;
			switch (type)
			{
				case SCType.Ship:
					jObject = new JShip(doc, file, destination);
					break;
				case SCType.Weapon:
					//jObject = new JWeapon(root, destination);
					throw new NotImplementedException();
					break;

				case SCType.None:
				default:
					throw new Exception($"Unknow CSType: '{file.FullName}', cannot determine the SCType");
			}
			// Start processing
			jObject?.Process();
		}

		/// <summary>
		/// Detect the type of the file
		/// </summary>
		/// <param name="filename">string: the targeted file</param>
		/// <returns></returns>
		public static SCType DetectType(string filename)
		{
			XmlDocument xml = new XmlDocument();
			xml.Load(filename);
			return DetectType(xml);
		}

		/// <summary>
		/// Detect the type of the file
		/// </summary>
		/// <param name="filename">XmlDocument: the targeted file</param>
		/// <returns></returns>
		private static SCType DetectType(XmlDocument xfile)
		{
			switch (xfile.SelectSingleNode("/*").Name)
			{
				case "Vehicle":
					return SCType.Ship;
				case "EntityClassDefinition.apar_special_ballistic_01":
					return SCType.Weapon;

				default:
					return SCType.None;
			}
		}
	}
}
