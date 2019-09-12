using SharedProject;
using System;
using System.Linq;
using System.IO;
using StarCitizen_JSON_to_SQL.Cry;

namespace StarCitizen_JSON_to_SQL
{
	class Program
	{
		public static bool debug { get; internal set; } = false;
		public static bool minify { get; internal set; } = false;

		public static DateTime starttime = DateTime.Now;
		public static string assembly_directory = AppContext.BaseDirectory;

		static void Main(string[] args)
		{
			Console.OutputEncoding = System.Text.Encoding.UTF8;
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			bool hasException = false;

			if (args.Length < 4 || args.Contains("-h") || args.Contains("--help"))
			{
				PrintHelp();
				return;
			}

			string working_dir = Environment.CurrentDirectory;

			args[0] = args[0].Trim(new char[] { '\'', '\"' });
			args[1] = args[1].Trim(new char[] { '\'', '\"' });
			args[2] = args[2].Trim(new char[] { '\'', '\"' });
			args[3] = args[3].Trim(new char[] { '\'', '\"' });

			string source = new DirectoryInfo(args[0]).FullName;
			string destination = args[1];
			string database_name = args[2];
			string version = args[3];

			SCType filters = FindParameters(args.Skip(destination == "./" ? 1 : 2).ToArray()); // skip source and destination from args

			Logger.LogEmpty("Process has started.");
			Logger.LogDebug("DEBUG MODE ENABLED");
			Logger.LogDebug("Arguments: " + String.Join(' ', args));
			Logger.LogEmpty();
			Logger.LogEmpty("Parameters:");
			Logger.LogEmpty($"\tSource:\t\t{source}");
			Logger.LogEmpty($"\tDestination:\t{destination}");
			Logger.LogEmpty($"\tDatabase:\t{database_name}");
			Logger.LogEmpty($"\tVersion:\t{version}");
			Logger.LogEmpty($"\tMinify:\t\t{(minify ? "Yes" : "No")}");

			if (debug)
			{
				Logger.LogEmpty($"Filter:");
				Logger.LogEmpty("\tShips: " + ((filters & SCType.Ship) == SCType.None ? "No" : "Yes"));
				Logger.LogEmpty("\tShops: " + ((filters & SCType.Shop) == SCType.None ? "No" : "Yes"));
				Logger.LogEmpty("\tWeapons: " + ((filters & SCType.Weapon) == SCType.None ? "No" : "Yes"));
				Logger.LogEmpty("\tStations: " + ((filters & SCType.None) == SCType.None ? "No" : "Yes"));
				Logger.LogEmpty("\tCommodities: " + ((filters & SCType.Commoditie) == SCType.None ? "No" : "Yes"));
				Logger.LogEmpty("\tManufacturers: " + ((filters & SCType.Manufacturer) == SCType.None ? "No" : "Yes"));
			}
			Logger.LogEmpty();

			if (filters == SCType.None)
			{
				Logger.LogInfo("No filter(s) entered, try to add a least one filter.");
				Logger.LogInfo("Type '--help' for help.");
				return;
			}

			Logger.Log("Loading directory.. ", end: "");
			Tuple<string, SCType>[] files = null;

#if DEBUG
			files = (Tuple<string, SCType>[])Progress.Process(() => GetFiles(source, filters), "Done");
#else
			try
			{
				files = (Tuple<string, SCType>[])Progress.Process(() => GetFiles(source, filters), "Done");
			}
			catch (Exception ex)
			{
				Logger.LogError("Loading directory.. FAILED", ex, start: "\r");
				Exit(true);
			}
#endif
			Logger.LogInfo("Starting..");
			CryJSON cryJson = new CryJSON(destination, database_name, version);

			var category = SCType.None;
			foreach (Tuple<string, SCType> file in new ProgressBar(files, "Converting", true))
			{
				FileInfo f = new FileInfo(file.Item1);
				if (category != file.Item2)
				{
					category = file.Item2;
					Logger.LogEmpty();
					Logger.LogInfo($"Category [{category.ToString()}]", clear_line: true);
				}

				Logger.Log($"Converting {f.Name}..  ", end: "");

#if DEBUG
				Progress.Process(() => cryJson.ConvertJSON(f, file.Item2), "Done");
#else
				try
				{
					Progress.Process(() => cryJson.ConvertJSON(f, file.Item2), "Done");
				}
				catch (Exception ex)
				{
					Logger.LogError($"Converting {f.Name}.. FAILED 🔥", start: "\r", exception: ex);
					hasException = true;
				}
#endif

			}

			Exit(hasException);
		}

		private static void PrintHelp()
		{
			Logger.LogEmpty("Usage: dotnet StarCitizen_JSON_to_SQL <source> <destination> <database name> <version> [CONFIG] [FILTER(S)]");
			Logger.LogEmpty("Convert any StarCitizen JSON files to SQL");
			Logger.LogEmpty();
			Logger.LogEmpty("[Required]");
			Logger.LogEmpty("\tsource: \tthe folder of JSONs.");
			Logger.LogEmpty("\tdestination: \tthe path of the .sql file.");
			Logger.LogEmpty("\tdatabase_name: \tthe name of the database.");
			Logger.LogEmpty("\tversion: \tthe name of the version.");
			Logger.LogEmpty();
			Logger.LogEmpty("[Config]");
			Logger.LogEmpty("\t--debug\t\tprint all Debug infos.");
			Logger.LogEmpty("\t\t\tdefault: no.");
			Logger.LogEmpty("\t--minify\t\tconvert JSON in a minified format.");
			Logger.LogEmpty("\t\t\tdefault: no.");
			Logger.LogEmpty("\t--help, -h\tprint this message.");
			Logger.LogEmpty();
			Logger.LogEmpty("[Filters]");
			Logger.LogEmpty("\t--all\t\t\tConvert every entities.");
			Logger.LogEmpty("\t--ships, -s\t\tConvert Ships.");
			Logger.LogEmpty("\t--weapons, -w\t\tConvert Weapons.");
			Logger.LogEmpty("\t--weapons-magazine, -wm\tConvert Weapons Magazines.");
			Logger.LogEmpty("\t--commodities, -c\tConvert Commodities.");
			Logger.LogEmpty("\t--tags, -t\t\tConvert Tags.");
			Logger.LogEmpty("\t--shops, -S\t\tConvert Shops.");
			Logger.LogEmpty("\t--manufacturers, -m\tConvert Manufacturers.");
			Logger.LogEmpty("\t--starmap, -sh\t\tConvert Starmap.");
		}

		/// <summary>
		/// Exit the application
		/// </summary>
		/// <param name="hasException"></param>
		private static void Exit(bool hasException)
		{
			Logger.LogInfo($"Execution time: {(DateTime.Now - starttime).TotalSeconds.ToString("00.00s")}");

			if (hasException)
			{
				Logger.LogEmpty();
				Logger.LogEmpty("=====================================");
				Logger.LogError("Something went wrong!");
				Logger.LogError($"More details can be found in: '{Path.Combine(assembly_directory, Logger.filename)}'");
				Logger.LogEmpty("=====================================");
			}
			Logger.LogEmpty();
			Logger.WriteLog();
			Environment.Exit(hasException ? 1 : 0);
		}

		/// <summary>
		/// Get all xml files in de directory and subdirectory
		/// </summary>
		/// <param name="source">Where to search for XML</param>
		/// <returns></returns>
		private static Tuple<string, SCType>[] GetFiles(string source, SCType filter)
		{
			try
			{
				return Directory.GetFiles(source, "*.json", SearchOption.AllDirectories)
					.Where(f => (filter & CryJSON.DetectType(f)) != SCType.None)
					.ToList()
					.ConvertAll(f => new Tuple<string, SCType>(f, CryJSON.DetectType(f)))
					.OrderBy(f => f.Item2)
					.ToArray();
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
			return new Tuple<string, SCType>[0];
		}

		/// <summary>
		/// Parse all args from terminal
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		private static SCType FindParameters(string[] args)
		{
			SCType parameters = SCType.None;

			foreach (string arg in args)
			{
				switch (arg)
				{
					case "--debug":
						debug = true;
						break;
					case "--minify":
						minify = true;
						break;


					case "--ships":
					case "-s":
						parameters |= SCType.Ship;
						break;

					case "--shops":
					case "-S":
						parameters |= SCType.Shop;
						break;

					case "--weapons":
					case "-w":
						parameters |= SCType.Weapon;
						break;

					case "--weapons-magazine":
					case "-wm":
						parameters |= SCType.Weapon_Magazine;
						break;

					case "--commodities":
					case "-c":
						parameters |= SCType.Commoditie;
						break;

					case "--manufacturer":
					case "-m":
						parameters |= SCType.Manufacturer;
						break;

					case "--all":
						parameters = SCType.Every;
						break;

					default:
						Logger.LogWarning("Unknown parameter: " + arg);
						Logger.LogInfo("Type '--help' for help.");
						Logger.LogEmpty();
						break;
				}
			}

#if DEBUG
			debug = true;
#endif
			Logger.debug = debug;
			return parameters;
		}
	}
}
