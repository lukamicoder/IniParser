using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace IniParser {
	public class Ini {
		private List<IniSection> _sectionList;
		private string _fileName;
		private string[] _lines;

		public Ini(string fileName) {
			_sectionList = new List<IniSection>();

			if (!File.Exists(fileName)) {
				var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

				if (!String.IsNullOrEmpty(path)) {
					fileName = Path.Combine(path, fileName);

					if (!File.Exists(fileName)) {
						throw new FileNotFoundException("Ini file not found at " + fileName + " or " + fileName);
					}
				}
			}

			_fileName = fileName;

			Parse();
		}

		private void Parse() {
			_lines = File.ReadAllLines(_fileName);

			for (int x = 0; x < _lines.Length; x++) {
				_lines[x] = _lines[x].Trim();
				string line = _lines[x];

				if (line.Length == 0 || line[0] == ';' || line[0] == '#' || line[0] == '\n' || line[0] == '\r') {
					continue;
				}

				int index;
				if (line[0] == '[') {
					index = line.IndexOf(']');
					if (index < 1) {
						throw new ParsingException(x + 1, line, "Failed to parse section header");
					}

					_sectionList.Add(new IniSection(line.Substring(1, index - 1)));

					continue;
				}

				if (_sectionList.Count == 0) {
					throw new ParsingException(x + 1, line, "Line is not under any section");
				}

				index = line.IndexOf('=');
				if (index < 1) {
					throw new ParsingException(x + 1, line, "Failed to parse line");
				}
				string key = line.Substring(0, index).Trim();
				string val = line.Substring(index + 1).Trim();

				if (val[0] == '\"' && val[val.Length - 1] == '\"') {
					val = val.Substring(1, val.Length - 2);
				}

				var section = _sectionList[_sectionList.Count - 1];
				section.Keys.Add(key, val);
			}
		}

		public IniSection GetSection(string sectionName) {
			return _sectionList.FirstOrDefault(s => s.Name == sectionName);
		}

		public List<IniSection> GetSections() {
			return _sectionList;
		}

		public string GetString(string sectionName, string key) {
			string val = null;

			var section = GetSection(sectionName);
			if (section != null) {
				section.Keys.TryGetValue(key, out val);
			}

			return val;
		}

		public bool GetBool(string sectionName, string key) {
			var val = GetString(sectionName, key);
			if (String.IsNullOrEmpty(val)) {
				return false;
			}

			if (val == "1" && val.ToLower() == "true") {
				return true;
			}

			return false;
		}

		public int GetInt(string sectionName, string key) {
			var val = GetString(sectionName, key);
			if (String.IsNullOrEmpty(val)) {
				return 0;
			}

			int value;
			if (!int.TryParse(val, out value)) {
				return 0;
			}

			return value;
		}

		public double GetDouble(string sectionName, string key) {
			var val = GetString(sectionName, key);
			if (String.IsNullOrEmpty(val)) {
				return 0;
			}

			double value;
			if (!double.TryParse(val, out value)) {
				return 0;
			}
			return value;
		}

		public void UpdateValue(string sectionName, string key, string val) {
			var section = GetSection(sectionName);
			if (section == null) {
				throw new ArgumentException("No section found: " + sectionName);
			}
			if (!section.Keys.ContainsKey(key)) {
				throw new ArgumentException("No key found in section " + sectionName + ": " + key);
			}

			section.Keys[key] = val;

			bool inSection = false;
			for (int x = 0; x < _lines.Length; x++) {
				if (_lines[x] == "[" + sectionName + "]") {
					inSection = true;
					continue;
				}

				if (_lines[x].Length > 0 && _lines[x][0] == '[') {
					inSection = false;
					continue;
				}

				if (inSection && _lines[x].Contains("=") && (_lines[x].Substring(0, _lines[x].IndexOf('='))).Trim() == key) {
					_lines[x] = key + "=" + val;
					break;
				}
			}

			File.WriteAllLines(_fileName, _lines);
		}

		public void UpdateValue(string sectionName, string key, bool val) {
			UpdateValue(sectionName, key, val ? "true" : "false");
		}

		public void UpdateValue(string sectionName, string key, int val) {
			UpdateValue(sectionName, key, val + "");
		}

		public void UpdateValue(string sectionName, string key, double val) {
			UpdateValue(sectionName, key, val + "");
		}
	}
}
