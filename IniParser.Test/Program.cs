using System;

namespace IniParser.Test {
	class Program {
		static void Main(string[] args) {
			new Program();
		}

		public Program() {
			var ini = Ini.Instance;

			var val = ini.GetInt("countString", "vm");
			Console.WriteLine(val);

			val = ini.GetInt("count", "vm");
			Console.WriteLine(val);

			var val1 = ini.GetString("count", "vm");
			Console.WriteLine(val);

			ini.UpdateValue("countString", 67, "vm");

			Console.ReadKey();
		}
	}
}
