using System;

namespace StarCitizen_XML_to_JSON
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: sc_xml_json.exe [source] <destination>");
                Console.WriteLine("Convert any StarCitizen XML files to JSON");
                Console.WriteLine();
                Console.WriteLine("[Required]");
                Console.WriteLine("\tsource: \tthe folder to extract XML data.");
                Console.WriteLine();
                Console.WriteLine("[Optional]");
                Console.WriteLine("\tdestination: \twrite all JSON in the destination, respecting source hierarchy.");
                Console.WriteLine("\t\t\tdefault: current working directory.");
                return;
            }

            string source = args[0];
            string destination = (args.Length == 2) ? args[1] : ".";

            Console.WriteLine(source);
            Console.WriteLine(destination);
        }
    }
}
