using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StarCitizen_XML_to_JSON
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1 || args.Contains("-h") || args.Contains("--help"))
            {
                Console.WriteLine("Usage: sc_xml_json.exe [source] <destination> [-s|-w|-S]");
                Console.WriteLine("Convert any StarCitizen XML files to JSON");
                Console.WriteLine();
                Console.WriteLine("[Required]");
                Console.WriteLine("\tsource: \tthe folder to extract XML data.");
                Console.WriteLine();
                Console.WriteLine("[Optional]");
                Console.WriteLine("\tdestination: \twrite all JSON in the destination, respecting source hierarchy.");
				Console.WriteLine("\t\t\tdefault: current working directory.");
				Console.WriteLine();
				Console.WriteLine("[Filters]");
				Console.WriteLine("\t--ships, -s\t: Convert Ships.");
				Console.WriteLine("\t--weapons, -w\t: Convert Weapons.");
				Console.WriteLine("\t--stations, -S\t: Convert Stations.");
                return;
            }

			string working_dir = Environment.CurrentDirectory;

			string source = new DirectoryInfo(args[0]).FullName;
            string destination = new DirectoryInfo((args.Length >= 2) ? args[1] : ".").FullName;
			SCType filters = FindParameters(args);

			Console.WriteLine("Parameters:");
			Console.WriteLine($"\tSource:\t\t{source}");
			Console.WriteLine($"\tDestination:\t{destination}");
			Console.WriteLine($"Filter:");
			Console.WriteLine("\tShips: " + ((filters & SCType.Ship) == SCType.None ? "No" : "Yes"));
			Console.WriteLine("\tWeapons: " + ((filters & SCType.Weapon) == SCType.None ? "No" : "Yes"));
			Console.WriteLine("\tStations: " + ((filters & SCType.None) == SCType.None ? "No" : "Yes"));
			Console.WriteLine();

			Console.WriteLine("[+] Loading directory..");
			var files = GetFiles(source, filters);
			Console.WriteLine($"Files to be converted: {files.Length}");
			Console.WriteLine();

			Console.WriteLine("[+] Starting..");
			CryXML cryXml = new CryXML(source, destination);
			foreach (var f in files)
			{
				Console.Write($"[+] Converting {f.Name}..");
				//try
				//{
					cryXml.ConvertJSON(f, filters);
					Console.WriteLine($"\r[+] Converting {f.Name} Done");
				//}
				//catch (Exception ex)
				//{
					//Console.WriteLine($"\r[-] Converting {f.Name} ERROR");
					//Console.WriteLine($"[!] {ex.Message}");
					//throw ex;
				//}
			}
		}

		/// <summary>
		/// Get all xml files in de directory and subdirectory
		/// </summary>
		/// <param name="source">Where to search for XML</param>
		/// <returns></returns>
		private static FileInfo[] GetFiles(string source, SCType filter)
		{
			return Directory.GetFiles(source, "*.xml", SearchOption.AllDirectories)
				.Where(f => (filter & CryXML.DetectType(f)) != SCType.None)
				.ToList()
				.ConvertAll(f => new FileInfo(f))
				.ToArray();
		}

		private static SCType FindParameters(string[] args)
		{
			SCType parameters = 0;

			foreach (string arg in args)
			{
				Console.WriteLine(arg);
				switch (arg)
				{
					case "--ships":
					case "-s":
						parameters |= SCType.Ship;
						break;

					case "--weapons":
					case "-w":
						parameters |= SCType.Weapon;
						break;
				}
			}
			return parameters;
		}
    }
}
