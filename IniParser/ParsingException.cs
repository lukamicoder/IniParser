using System;

namespace IniParser {
    public class ParsingException : Exception {
        public ParsingException(int lineNumber, string line, string msg) : base(string.Format("Error \'{2}\' while parsing line {0}: \'{1}\'", lineNumber, line, msg)) {}
    }
}