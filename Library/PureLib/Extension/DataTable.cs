// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataTable.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the System_Data_DataTable type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Data;
using System.Linq;

using Regulus.Utility;

#endregion

namespace Regulus.Extension
{
	public static class System_Data_DataTable
	{
		public class DataTableType
		{
			public static readonly DataTableType[] Types =
			{
				new DataTableType("int", typeof (int), "0"), 
				new DataTableType("float", typeof (float), "0.0"), 
				new DataTableType("string", typeof (string), string.Empty)
			};

			public string Name { get; private set; }

			public Type Type { get; private set; }

			public string Default { get; private set; }

			public DataTableType(string name, Type type, string def_value)
			{
				this.Name = name;
				this.Type = type;
				this.Default = def_value;
			}
		}

		public static void ReadCSV(this DataTable dt, string path, string token)
		{
			var iterator = CSV.Read(path, token).GetEnumerator();

			// System.Data.DataTable dt = new System.Data.DataTable();
			// 第一行
			// iterator.MoveNext();
			// 第二行
			iterator.MoveNext();
			var rowNames = iterator.Current;

			// 第三行
			iterator.MoveNext();
			var rowTypes = iterator.Current;

			if (rowTypes.Fields.Length != rowNames.Fields.Length)
			{
				throw new Exception(string.Format("名稱與型別數量不符: RowNameCount={0},RowTypeCount={1}", rowNames.Fields.Length, 
					rowTypes.Fields.Length));
			}

			var defaultValues = new string[rowTypes.Fields.Length];
			for (var i = 0; i < rowNames.Fields.Length; ++i)
			{
				var name = rowNames.Fields[i];
				var strType = rowTypes.Fields[i];
				var result = (from ddt in DataTableType.Types where ddt.Name == strType select ddt).FirstOrDefault();
				defaultValues[i] = result.Default;
				if (result != null)
				{
					var dc = new DataColumn(name, result.Type);

					dt.Columns.Add(dc);
				}
				else
				{
					throw new Exception("無效的型別:" + strType);
				}
			}

			var rowCount = rowNames.Fields.Length;
			while (iterator.MoveNext())
			{
				var row = iterator.Current;
				var fields = row.Fields;

				if (fields.Length == rowCount)
				{
					var dtRow = dt.NewRow();
					var i = 0;
					foreach (var name in rowNames.Fields)
					{
						var field = fields[i] == string.Empty
							? defaultValues[i]
							: fields[i];
						dtRow[name] = field;
						++i;
					}

					dt.Rows.Add(dtRow);
				}
				else
				{
					throw new Exception(string.Format("資料欄位與名稱數量不符: Index={0},RowCount={1},FieldCount={2}", row.Index, rowCount, 
						fields.Length));
				}
			}
		}
	}
}