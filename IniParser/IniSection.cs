using System.Collections.Generic;

namespace IniParser {
    public class IniSection {
        public string Name { get; set; }
        public Dictionary<string, string> Keys { get; set; }

        public IniSection(string name) {
            Name = name;
            Keys = new Dictionary<string, string>();
        }
    }
}