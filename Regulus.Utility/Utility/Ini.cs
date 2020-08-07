using System.Collections.Generic;
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
            Dictionary<string, Dictionary<string, string>> dic = new Dictionary<string, Dictionary<string, string>>();


            foreach (Match sectionMatch in _GetSectionMatches(data))
            {
                string section = sectionMatch.Groups[1].Value;
                string records = sectionMatch.Groups[2].Value;
                IEnumerable<Match> recordMatches = _GetRecordMatches(records);
                foreach (Match recordMatch in recordMatches)
                {
                    string key = recordMatch.Groups[1].Value;
                    string value = recordMatch.Groups[2].Value;

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
            Regex commitRegex = new Regex(@"^\s*([\w\s]+?)\s*=\s*([^=]+?)\s*$", RegexOptions.Compiled | RegexOptions.Multiline);

            foreach (Match match in commitRegex.Matches(records))
            {
                yield return match;
            }
        }



        IEnumerable<Match> _GetSectionMatches(string data)
        {
            Regex sectionRegex = new Regex(@"^\[\s*(.+?)\s*\]\s+([^[]+)", RegexOptions.Compiled | RegexOptions.Multiline);
            foreach (Match match in sectionRegex.Matches(data))
            {
                yield return match;
            }
        }


    }
}
