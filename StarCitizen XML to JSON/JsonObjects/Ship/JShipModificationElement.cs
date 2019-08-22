using System;

namespace StarCitizen_XML_to_JSON.JsonObjects.Ship
{
	public class JShipModificationElement
	{
        string idRef = null;
        string name = null;
        object value = null;

		public JShipModificationElement(string idRef, string name, string value)
		{
			this.idRef = idRef;
			this.name = name;
			this.value = value;
		}
	}
}
