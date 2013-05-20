using System;
using System.Collections.Generic;
using System.IO;

namespace Samebest.Utility
{

	class CSV
	{
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


