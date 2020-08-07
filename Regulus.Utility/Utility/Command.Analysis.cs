using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Regulus.Utility
{




    public partial class Command
    {
        public class Analysis
        {
            public string Command { get; private set; }

            public string[] Parameters { get; private set; }

            public Analysis(string message)
            {
                _Analyze(message);
            }

            private void _Analyze(string message)
            {
                string expansion = @"^\s*(?<command>\w+-?\d*\.?\w*)\s*\[\s*(?<args>.+?)\]\s*\[\s*(?<ret>.+?)\s*\]|^\s*(?<command>\w+-?\d*\.?\w*)\s*\[\s*(?<args>.+?)\]|^\s*(?<command>\w+-?\d*\.?\w*)\s*";
                Regex regex = new Regex(expansion);
                Match match = regex.Match(message);
                if (match.Success)
                {
                    Group command = match.Groups["command"];
                    Command = command.Value;
                    Group args = match.Groups["args"];
                    Group ret = match.Groups["ret"];
                    _SetParameters(_AnalyzeArgs(args.Value), _AnalyzeReturn(ret.Value));
                }
            }




            private void _SetParameters(string[] parameters, string return_parameter)
            {
                Parameters = parameters;
                Return = return_parameter;

            }

            public string Return { get; private set; }

            private string _AnalyzeReturn(string value)
            {
                return value;
            }

            private string[] _AnalyzeArgs(string message)
            {
                List<string> args = new List<string>();

                // \s*(\w+)\s*,?
                // ^\s*(?<command>\w+)\s*\[\s*(?<args>.+)\]|^\s*(?<command>\w+)\s*
                const string expansion = @"\s*(?<Arg>\w+)\s*,?";
                Regex regex = new Regex(expansion);
                MatchCollection matchs = regex.Matches(message);
                foreach (Match match in matchs)
                {
                    args.Add(match.Groups["Arg"].Value);
                }

                return args.ToArray();
            }
        }


    }
}
