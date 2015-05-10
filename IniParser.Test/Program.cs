using System;

namespace IniParser.Test {
	class Program {
		static void Main(string[] args) {
			var ini = new Ini("config.ini");

			var val = ini.GetInt("vm", "countString");
			Console.WriteLine(val);

			val = ini.GetInt("vm", "count");
			Console.WriteLine(val);

			var val1 = ini.GetString("vm", "count");
			Console.WriteLine(val);

			ini.UpdateValue("vm", "countString", 67);

			Console.ReadKey();
		}
	}
}
