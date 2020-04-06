using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System;
using System.Linq;

namespace StarCitizen_GameDataTests
{
    [TestClass]
    public class UnitTest
    {
		private const int Expected = 0;

		[TestMethod]
        public void TestXML_To_JSON()
		{
			string[] args = new string[] {
				@"P:\Star Citizen Dev\[3.8.1]\Data\",
				@"P:\Star Citizen Dev\[3.8.1]\Data.json\",
				"--weapons"
			};
			using (var sw = new StringWriter())
			{
				Console.SetOut(sw);
				StarCitizen_XML_to_JSON.Program.Main(args);
				
				var result = sw.ToString().Trim();
				if(true)
					Assert.Fail($"'StarCitizen_XML_to_JSON'\n{result}");
			}
		}

		[TestMethod]
		public void TestJSON_To_XML()
		{
			string[] args = new string[] {
				@"P:\Star Citizen Dev\[3.8.1]\Data.json",
				@"P:\Star Citizen Dev\[3.8.1]\database.sql",
				"starcitizen-gamedata",
				"3.8.1",
				"--all",
				"--minify"
			};
			using (var sw = new StringWriter())
			{
				Console.SetOut(sw);
				StarCitizen_JSON_to_SQL.Program.Main(args);

				var result = sw.ToString().Trim();
				if (true)
					Assert.Fail($"'StarCitizen_XML_to_JSON' \n{result}");
			}
		}
	}
}
