using System;
using System.Collections.Generic;
using System.Text;

namespace StarCitizen_XML_to_JSON
{
	public enum SCType
	{
		None			= 0,		// 0
		Ship			= 1 << 1,	// 1
		Weapon			= 1 << 2,	// 2
		Commoditie		= 1 << 3,	// 4
		Tag				= 1 << 4,	// 8
		Shop			= 1 << 5,	// 16
		Manufacturer	= 1 << 6,	// 32
		Starmap			= 1 << 7,	// 64

		Every			= (Starmap * 2) - 1,
	}
}
