using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Regulus.Utility
{
	public class Ini
	{
		private readonly Dictionary<string, Dictionary<string, string>> _Data;

		public Ini(string stream)
		{
			_Data = _Build(stream);
		}

		public void Write(string Section, string Key, string Value)
		{
			_Data[Section][Key] = Value;
		}

		public string Read(string Section, string Key)
		{
			return _Data[Section][Key];
		}

        public bool TryRead(string section, string key, out string value)
        {
            Dictionary<string, string> keys;
            if (_Data.TryGetValue(section, out keys))
                return keys.TryGetValue(key, out value);

            value = null;
            return false;
        }

	    private Dictionary<string, Dictionary<string, string>> _Build(string data)
		{
			var pattern = @"
^(?:\[)                  # Section Start
    (?<Section>[^\]]*)   # Actual Section text into Section Group
 (?:\])                  # Section End then EOL/EOB
     (?:[\r\n]{0,2}|\Z)  # Match but don't capture the CRLF or EOB

 (?<KVPs>                # Begin working on the Key Value Pairs
   (?!\[)                # Stop if a [ is found
   (?<Key>[^=]*?)        # Any text before the =, matched few as possible
      (?:=)              # Anchor the = now but don't capture it
   (?<Value>[^=\[]*)     # Get everything that is not an =
   (?=^[^=]*?=|\Z|\[)    # Look Ahead, Capture but don't consume
 )+                      # Multiple values
";

			var inifile
				= (from Match m in Regex.Matches(data, pattern, RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline)
				   select new
				   {
					   Section = m.Groups["Section"].Value, 
					   kvps = (from cpKey in m.Groups["Key"].Captures.Cast<Capture>().Select(
						   (a, i) => new
						   {
							   a.Value, 
							   i
						   })
					           join cpValue in m.Groups["Value"].Captures.Cast<Capture>().Select(
						           (b, i) => new
						           {
							           b.Value, 
							           i
						           }) on cpKey.i equals cpValue.i
					           select new KeyValuePair<string, string>(cpKey.Value, cpValue.Value)).ToDictionary(
						           kvp => kvp.Key.Trim(), 
						           kvp => kvp.Value.Trim())
				   }).ToDictionary(itm => itm.Section, itm => itm.kvps);

			return inifile;
		}

	    
	}
}
