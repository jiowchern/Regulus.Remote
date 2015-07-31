// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Csv.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the CSV type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

#endregion

namespace Regulus.Utility
{
	public class CSV<T>
	{
		private readonly PropertyInfo[] _Properties;

		private readonly string[] _Propertys;

		private readonly char[] _Separator;

		public CSV(string head, string separator)
		{
			var type = typeof (T);
			this._Properties = type.GetProperties();
			this._Separator = separator.ToCharArray();
			this._Propertys = (from line in this._Parse(head) select line.Trim()).ToArray();
		}

		private string[] _Parse(string row)
		{
			return row.Split(this._Separator, StringSplitOptions.RemoveEmptyEntries);
		}

		public void Parse(string row, ref T value)
		{
			var cols = this._Parse(row);

			if (cols.Length != this._Propertys.Length)
			{
				throw new Exception("csv parse error , Col Length != Property Length.");
			}

			for (var i = 0; i < cols.Length; ++i)
			{
				var name = this._Propertys[i];

				var info = this._Find(name);
				if (info.CanWrite == false)
				{
					throw new Exception(string.Format("csv parse error , Property {0} can't write.", name));
				}

				if ((info.MemberType | MemberTypes.Property) == 0)
				{
					throw new Exception(string.Format("csv parse error , Member {0} is not propert."));
				}

				info.SetValue(value, this._ParseValue(cols[i], info.PropertyType), null);
			}
		}

		private object _ParseValue(string value, Type type)
		{
			return Convert.ChangeType(value, type);
		}

		private PropertyInfo _Find(string name)
		{
			return (from info in this._Properties where info.Name == name select info).Single();
		}
	}

	public class CSV
	{
		public class Row
		{
			public int Index { get; private set; }

			public string[] Fields { get; private set; }

			public Row(int idx, string[] fields)
			{
				this.Index = idx;
				this.Fields = fields;
			}
		}

		public static T[] Parse<T>(string text, string separator, string paragraph)
			where T : new()
		{
			var values = new List<T>();
			var lines = CSV._ToLine(text, paragraph);
			if (lines.Length == 0)
			{
				throw new Exception("CSV Parse error , text lines 0");
			}

			var csv = new CSV<T>(lines[0], separator);
			for (var i = 1; i < lines.Length; ++i)
			{
				var row = lines[i];
				var value = new T();
				try
				{
					csv.Parse(row, ref value);
				}
				catch (Exception e)
				{
					var ex = new Exception(string.Format("cav parse error at Col{0}", i), e);

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

		public static IEnumerable<Row> Read(string path, string token)
		{
			var streamReader = new StreamReader(path);
			var allText = streamReader.ReadToEnd();
			streamReader.Close();

			/*System.Text.RegularExpressions.Regex regLine = new System.Text.RegularExpressions.Regex(@"[^\r\n]+[\r\n]");
            System.Text.RegularExpressions.Regex regField = new System.Text.RegularExpressions.Regex(@"([^\t\n\r]+)[\r\n]|([^\t\n\r]*)[\t]");
            System.Text.RegularExpressions.MatchCollection lines = regLine.Matches(text);*/
			var textArr = allText.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			string[] descArr = null;
			CSV.TryParseLine(textArr[0], ref descArr);

			var i = 0;
			var lineText = string.Empty;
			foreach (var text in textArr)
			{
				if (i == 0)
				{
					i++;
					continue;
				}

				lineText += text;
				string[] stringFields = null;
				if (CSV.TryParseLine(lineText, ref stringFields, descArr.Length))
				{
					lineText = string.Empty;
					yield return new Row(i++, stringFields);
				}
				else
				{
					lineText += "\\n";
				}
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
		private static bool TryParseLine(string text, ref string[] resArr, int refLength = 0)
		{
			resArr = text.Split('\t');
			if (refLength != 0 && resArr.Length < refLength)
			{
				return false;
			}

			for (var i = 0; i < resArr.Length; i++)
			{
				resArr[i] = resArr[i].Trim('"');
			}

			return true;
		}
	}
}