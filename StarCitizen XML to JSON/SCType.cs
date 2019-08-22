using System;
using System.Collections.Generic;
using System.Text;

namespace StarCitizen_XML_to_JSON
{
	public enum SCType
	{
		None	= 0b000, // 0
		Ship	= 0b001, // 1
		Weapon	= 0b010, // 2
		Every	= 0b111, // 7
	}
}
