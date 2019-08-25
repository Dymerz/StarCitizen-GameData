﻿using System;
using System.IO;
using System.Linq;

namespace StarCitizen_XML_to_JSON
{
    class Program
	{
		public static bool debug { get; internal set; } = false;

		static void Main(string[] args)
		{
			Console.OutputEncoding = System.Text.Encoding.UTF8;

			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			bool hasException = false;

			if (args.Length < 1 || args.Contains("-h") || args.Contains("--help"))
			{
				Logger.LogEmpty("Usage: sc_xml_json.exe [source] <destination> [-s|-w|-S]");
				Logger.LogEmpty("Convert any StarCitizen XML files to JSON");
				Logger.LogEmpty();
				Logger.LogEmpty("[Required]");
				Logger.LogEmpty("\tsource: \tthe folder to extract XML data.");
				Logger.LogEmpty();
				Logger.LogEmpty("[Optional]");
				Logger.LogEmpty("\tdestination: \twrite all JSON in the destination, respecting source hierarchy.");
				Logger.LogEmpty("\t\t\tdefault: current working directory.");
				Logger.LogEmpty();
				Logger.LogEmpty("[Filters]");
				Logger.LogEmpty("\t--ships, -s\t: Convert Ships.");
				Logger.LogEmpty("\t--weapons, -w\t: Convert Weapons.");
				Logger.LogEmpty("\t--stations, -S\t: Convert Stations.");
				return;
			}

			string working_dir = Environment.CurrentDirectory;

			string source = new DirectoryInfo(args[0]).FullName + "\\";
			string destination = new DirectoryInfo((args.Length >= 2) ? args[1] : ".").FullName + "\\";
			SCType filters = FindParameters(args);

			Logger.LogEmpty("Process has started.");
			Logger.LogDebug("DEBUG MODE ENABLED");
			Logger.LogEmpty("Parameters:");
			Logger.LogEmpty($"\tSource:\t\t{source}");
			Logger.LogEmpty($"\tDestination:\t{destination}");
			Logger.LogEmpty($"Filter:");
			Logger.LogEmpty("\tShips: " + ((filters & SCType.Ship) == SCType.None ? "No" : "Yes"));
			Logger.LogEmpty("\tWeapons: " + ((filters & SCType.Weapon) == SCType.None ? "No" : "Yes"));
			Logger.LogEmpty("\tStations: " + ((filters & SCType.None) == SCType.None ? "No" : "Yes"));
			Logger.LogEmpty();

			if (filters == SCType.None)
			{
				Logger.LogInfo("No filter(s) entered,  try to add a least one filter.");
				Logger.LogInfo("Type '--help' for help.");
				return;
			}

			Logger.Log("Loading directory.. ", end: "");
			var files = (string[])Progress.Process(() => GetFiles(source, filters), "Done");

			Logger.Log("Preparing resources.. ", end: "");
			CryXML cryXml = (CryXML)Progress.Process(() => new CryXML(source, destination), "Done");

			Logger.LogInfo($"Files to be converted: {files.Length}");
			Logger.LogEmpty();
			Logger.Log("Starting..");

			foreach (var file in files)
			{
				FileInfo f = new FileInfo(file);

				Logger.Log($"Converting {f.Name}.. ", end: "");

				// catch exception on Release build
#if RELEASE
				try
				{
#endif
					Progress.Process(() => cryXml.ConvertJSON(f, filters), "Done");
					throw new Exception();
#if RELEASE
				}
				catch (Exception ex)
				{
					Logger.LogError($"Converting {f.Name}.. FAILED 🔥", start: "\r", exception: ex);
					hasException = true;
				}
#endif
			}

			if (hasException)
			{
				Logger.LogEmpty("=====================================");
				Logger.LogError("Something went wrong!");
				Logger.LogError($"More details can be found in: '{Environment.CurrentDirectory + "/" + Logger.filename}'");
				Logger.LogEmpty("=====================================");
			}

			Logger.WriteLog();
		}

		/// <summary>
		/// Get all xml files in de directory and subdirectory
		/// </summary>
		/// <param name="source">Where to search for XML</param>
		/// <returns></returns>
		private static string[] GetFiles(string source, SCType filter)
		{
			try
			{
				return Directory.GetFiles(source, "*.xml", SearchOption.AllDirectories)
				.Where(f => 
					!f.ToLower().EndsWith("game.xml") && 
					(filter & CryXML.DetectType(f)) != SCType.None)
				.ToList()
				//.ConvertAll(f => new FileInfo(f))
				.ToArray();
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
			return new string[0];
		}

		private static SCType FindParameters(string[] args)
		{
			SCType parameters = SCType.None;

			foreach (string arg in args)
			{
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

					case "--debug":
						debug = true;
						break;

					default:
						parameters = SCType.None;
						break;
				}
			}

#if DEBUG
			debug = true;
#endif
			return parameters;
		}
    }
}
