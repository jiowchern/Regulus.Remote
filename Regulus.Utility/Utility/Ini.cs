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
	        var dic = new Dictionary<string, Dictionary<string, string>>();


            foreach (var sectionMatch in _GetSectionMatches(data))
		    {
		        var section = sectionMatch.Groups[1].Value;
		        var records = sectionMatch.Groups[2].Value;
		        var recordMatches = _GetRecordMatches(records);
		        foreach (var recordMatch in recordMatches)
		        {
		            var key = recordMatch.Groups[1].Value;
		            var value = recordMatch.Groups[2].Value;

		            if (!dic.ContainsKey(section))
		            {
		                dic.Add(section, new Dictionary<string, string>());
                    }		            

		            dic[section].Add(key, value);
                }
		    }
            
	        return dic;
	    }

	    

        private IEnumerable<Match> _GetRecordMatches(string records)
	    {
            var commitRegex = new Regex(@"^\s*([\w\s]+?)\s*=\s*([^=]+?)\s*$", RegexOptions.Compiled| RegexOptions.Multiline);
             
	        foreach (Match match in commitRegex.Matches(records))
	        {
	            yield return match;
	        }
	    }

	    

	    IEnumerable<Match> _GetSectionMatches(string data)
	    {
	        var sectionRegex = new Regex(@"^\[\s*(.+?)\s*\]\s+([^[]+)", RegexOptions.Compiled | RegexOptions.Multiline);
	        foreach (Match match in sectionRegex.Matches(data))
	        {
	            yield return match;
	        }
        }

	    
	}
}
