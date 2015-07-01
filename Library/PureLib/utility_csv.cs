using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Regulus.Utility
{

    public class CSV<T>
	{
        
        private string[] _Propertys;

        char[] _Separator;
        private System.Reflection.PropertyInfo[] _Properties;

        public CSV(string head, string separator)
        {
            var type = typeof(T);
            _Properties = type.GetProperties();
            _Separator = separator.ToCharArray();
            _Propertys = (from line in _Parse(head)  select line.Trim()).ToArray();
        }

        private string[] _Parse(string row)
        {
            
            return row.Split(_Separator, StringSplitOptions.RemoveEmptyEntries) ;
        }

        public void Parse(string row ,ref T value)
        {            
            var cols = _Parse(row);

            if (cols.Length != _Propertys.Length)
                throw new System.Exception("csv parse error , Col Length != Property Length.");

            for(int i = 0 ; i < cols.Length ; ++i)
            {
                var name = _Propertys[i];

                System.Reflection.PropertyInfo info = _Find(name);
                if(info.CanWrite == false)
                    throw new System.Exception(string.Format("csv parse error , Property {0} can't write." , name) );
                if( (info.MemberType | System.Reflection.MemberTypes.Property) == 0)
                    throw new System.Exception(string.Format("csv parse error , Member {0} is not propert."));

                info.SetValue(value, _ParseValue(cols[i] , info.PropertyType ), null);
            }            
        }        

        private object _ParseValue(string value , Type type)
        {
            return Convert.ChangeType(value, type);
        }

        private System.Reflection.PropertyInfo _Find(string name)
        {
            return (from info in _Properties where info.Name == name select info).Single();
        }


        
    }

    public class CSV
    {
        public static T[] Parse<T>(string text, string separator, string paragraph) 
            where T : new()
        {
            List<T> values = new List<T>();
            string[] lines = _ToLine(text, paragraph);
            if(lines.Length == 0)
            {
                throw new System.Exception("CSV Parse error , text lines 0");
            }

            var csv = new CSV<T>(lines[0], separator);
            for(int i = 1 ; i < lines.Length ; ++i)            
            {
                var row = lines[i];
                T value = new T();
                try
                {
                    csv.Parse(row, ref value);
                }
                catch(System.Exception e)
                {
                    var ex = new System.Exception(string.Format("cav parse error at Col{0}" , i ) ,  e);
                    
                    throw ex;
                }
                values.Add(value);
            }

            return values.ToArray();

        }

        private static string[] _ToLine(string text, string paragraph)
        {
            return text.Split(paragraph.ToCharArray());
        }

        public class Row
        {
            public Row(int idx, string[] fields)
            {
                Index = idx;
                Fields = fields;
            }
            public int Index { get; private set; }
            public string[] Fields { get; private set; }

        }
        public static IEnumerable<Row> Read(string path, string token)
        {
            StreamReader streamReader = new StreamReader(path);
            string allText = streamReader.ReadToEnd();
            streamReader.Close();

            /*System.Text.RegularExpressions.Regex regLine = new System.Text.RegularExpressions.Regex(@"[^\r\n]+[\r\n]");
            System.Text.RegularExpressions.Regex regField = new System.Text.RegularExpressions.Regex(@"([^\t\n\r]+)[\r\n]|([^\t\n\r]*)[\t]");
            System.Text.RegularExpressions.MatchCollection lines = regLine.Matches(text);*/

            string[] textArr = allText.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string[] descArr = null;
            TryParseLine(textArr[0], ref descArr);

            int i = 0;
            string lineText = "";
            foreach (var text in textArr)
            {
                if (i == 0)
                {
                    i++;
                    continue;
                }
                lineText += text;
                string[] stringFields = null;
                if (TryParseLine(lineText, ref stringFields, descArr.Length))
                {
                    lineText = "";
                    yield return new Row(i++, stringFields);
                }
                else
                    lineText += "\\n";
            }

            /*int i = 0;
            foreach (var line in lines)
            {
                System.Text.RegularExpressions.MatchCollection fields = regField.Matches(line.ToString());

                string[] stringFields = fields.Cast<System.Text.RegularExpressions.Match>().Select(m =>
                {
                    if (m.Groups[1].Value != "")
                        return m.Groups[1].Value;
                    if (m.Groups[2].Value != "")
                        return m.Groups[2].Value;
                    return "";
                }).ToArray<string>();

                yield return new Row(i++, stringFields);
            }*/
        }
        // Try Parse line
        static bool TryParseLine(string text, ref string[] resArr, int refLength = 0)
        {
            resArr = text.Split(new char[] { '\t' });
            if (refLength != 0 && resArr.Length < refLength)
                return false;

            for (int i = 0; i < resArr.Length; i++)
                resArr[i] = resArr[i].Trim(new char[] { '"' });
            return true;
        }
    }
}



