namespace SharedProject
{
	public enum SCType
	{
		None			= 0,		// 0
		Ship			= 1 << 1,	// 1
		Weapon			= 1 << 2,	// 2
		Weapon_Magazine	= 1 << 3,   // 4
		Commodity		= 1 << 4,	// 8
		//Tag				= 1 << 5,	// 16
		Shop			= 1 << 6,	// 32
		Manufacturer	= 1 << 7,	// 64
		//Starmap			= 1 << 8,	// 128

		Every			= (Manufacturer * 2) - 1,
	}
}
