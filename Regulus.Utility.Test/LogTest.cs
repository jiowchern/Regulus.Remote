using Regulus.Utility;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RegulusLibraryTest
{

    public class LogTest
    {
        [Xunit.Fact]
        public void TestWrite()
        {
            List<string> messages = new List<string>();

            Singleton<Log>.Instance.RecordEvent += message =>
            {
                lock (messages)
                    messages.Add(message);
            };

            Singleton<Log>.Instance.WriteInfo("123");
            Singleton<Log>.Instance.WriteInfo("456");
            Singleton<Log>.Instance.WriteInfo("789");
            Singleton<Log>.Instance.WriteInfo("123");
            Singleton<Log>.Instance.WriteInfo("1");

            while (true)
            {
                lock (messages)
                {
                    if (messages.Count >= 5)
                    {
                        break;
                    }
                }
            }

            string r1 = Regex.Match(messages[0], @"\[\d+/\d+/\d+_\d+:\d+:\d+\]\[Info\](\d+)").Groups[1].Value;
            string r2 = Regex.Match(messages[1], @"\[\d+/\d+/\d+_\d+:\d+:\d+\]\[Info\](\d+)").Groups[1].Value;
            string r3 = Regex.Match(messages[2], @"\[\d+/\d+/\d+_\d+:\d+:\d+\]\[Info\](\d+)").Groups[1].Value;
            string r4 = Regex.Match(messages[3], @"\[\d+/\d+/\d+_\d+:\d+:\d+\]\[Info\](\d+)").Groups[1].Value;
            string r5 = Regex.Match(messages[4], @"\[\d+/\d+/\d+_\d+:\d+:\d+\]\[Info\](\d+)").Groups[1].Value;


            Xunit.Assert.Equal("123", r1);
            Xunit.Assert.Equal("456", r2);
            Xunit.Assert.Equal("789", r3);
            Xunit.Assert.Equal("123", r4);
            Xunit.Assert.Equal("1", r5);

        }
    }
}
