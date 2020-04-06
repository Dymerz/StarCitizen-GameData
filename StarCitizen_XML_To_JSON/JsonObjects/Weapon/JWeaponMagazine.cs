using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace StarCitizen_XML_to_JSON.JsonObjects.Weapon
{
	class JWeaponMagazine : JWeapon
	{
		internal override string directory_name { get => "WeaponsMagazine"; }

		public JWeaponMagazine(FileInfo file, string destination, string source) : base(file, destination, source) { }

		/// <summary>
		/// Process the XML conversion
		/// </summary>
		public override void Process()
		{
			var root = doc.SelectSingleNode("/*"); // get the root element

			var manufacturer = base.LoadManufacturer(root);
			var ammoParams = base.LoadAmmoParams(root);

			if (manufacturer != null)
			{
				var manufacturer_imported = doc.ImportNode(manufacturer, true);
				doc.SelectSingleNode("*/Components/SAttachableComponentParams/AttachDef").AppendChild(manufacturer_imported);
			}

			if (ammoParams != null)
			{
				var ammoParams_imported = doc.ImportNode(ammoParams, true);
				doc.SelectSingleNode("*/Components/SAmmoContainerComponentParams").AppendChild(ammoParams_imported);
			}

			var name = root.Name.Split(".")[1];
			base.WriteFile(doc, name); // write the main ship
		}
	}
}

