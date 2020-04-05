using SharedProject;
using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace StarCitizen_JSON_to_SQL.Cry
{
	class CryJSON
	{
		private string destination;
		private string database_name;
		private string version;
		private string[] categories;

		public CryJSON(string destination, string database_name, string version)
		{
			this.destination = destination ?? throw new ArgumentNullException(nameof(destination));
			this.database_name = database_name ?? throw new ArgumentNullException(nameof(database_name));
			this.version = version ?? throw new ArgumentNullException(nameof(version));

			if (!File.Exists(destination))
				File.Create(destination).Close();
			else
				File.WriteAllText(destination, "");

			ConvertCategory();
		}

		public void ConvertJSON(FileInfo f, SCType type)
		{
			string sql_format = "INSERT IGNORE INTO `{0}`.`gamedata`(`category_id`, `version`, `name`, `data`) VALUES ({1}, '{2}', '{3}', '{4}');";

			string content = File.ReadAllText(f.FullName);

			if (Program.minify)
			{
				content = Regex.Replace(content, "(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", "$1");
				content = Regex.Replace(content, "\\\\", "\\\\");
				content = Regex.Replace(content, @"\\\\'", "\\\'");
				content = Regex.Replace(content, "\t", "\\\\t");
			}

			string name = f.Name.Replace(".json", "").Replace("_", " ");

			using (StreamWriter writer = new StreamWriter(destination, true))
			{
				writer.WriteLine(
					String.Format(sql_format, 
						database_name, getCategoryId(type), version, name, content));
			}
		}

		public void ConvertCategory()
		{
			categories = Enum.GetNames(typeof(SCType));


			string sql_format = "INSERT IGNORE INTO `{0}`.`category`(`id`, `value`, `name`, `label`) VALUES ({1}, {2}, '{3}', '{4}');";
			using (StreamWriter writer = new StreamWriter(destination, true))
			{
				foreach (var cat in categories)
				{
					if (cat == "None" || cat == "Every")
						continue;

					string label = cat.ToLower();
					label = label.Replace("_", " ").Trim();
					if (label.Contains(" "))
					{
						label = String.Join(' ', label.Split(" ")
						.ToList()
						.Select((l) => l = l[0].ToString().ToUpper() + l.Substring(1)));
					}
					else
						label = label[0].ToString().ToUpper() + label.Substring(1);

					int index = getCategoryId((SCType)Enum.Parse(typeof(SCType), cat));
					writer.WriteLine(
						String.Format(sql_format, database_name, index, (int)Enum.Parse(typeof(SCType), cat), cat.ToLower(), label));
				}
			}
		}

		private int getCategoryId(SCType type)
		{
			return (int)Math.Log((int)type, 2);
		}

		public static SCType DetectType(string file)
		{
			switch (new FileInfo(file).Directory.Name.ToLower())
			{
				case "commodities":
					return SCType.Commodity;
				case "manufacturers":
					return SCType.Manufacturer;
				case "ships":
					return SCType.Ship;
				case "shops":
					return SCType.Shop;
				case "weapons":
					return SCType.Weapon;
				case "weaponsmagazine":
					return SCType.Weapon_Magazine;

				default:
					return SCType.None;
			}
		}
	}
}
