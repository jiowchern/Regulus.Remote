using Regulus.Utility;
using System;
using System.Data;
using System.Linq;

namespace Regulus.Extension
{
    public static class System_Data_DataTable
    {
        public class DataTableType
        {
            public static readonly DataTableType[] Types =
            {
                new DataTableType("int", typeof(int), "0"),
                new DataTableType("float", typeof(float), "0.0"),
                new DataTableType("string", typeof(string), string.Empty)
            };

            public string Name { get; private set; }

            public Type Type { get; private set; }

            public string Default { get; private set; }

            public DataTableType(string name, Type type, string def_value)
            {
                Name = name;
                Type = type;
                Default = def_value;
            }
        }

        public static void ReadCSV(this DataTable dt, string path, string token)
        {
            System.Collections.Generic.IEnumerator<CSV.Row> iterator = CSV.Read(path, token).GetEnumerator();

            // System.Data.DataTable dt = new System.Data.DataTable();
            // 第一行
            // iterator.MoveNext();
            // 第二行
            iterator.MoveNext();
            CSV.Row rowNames = iterator.Current;

            // 第三行
            iterator.MoveNext();
            CSV.Row rowTypes = iterator.Current;

            if (rowTypes.Fields.Length != rowNames.Fields.Length)
            {
                throw new Exception(
                    string.Format(
                        "名稱與型別數量不符: RowNameCount={0},RowTypeCount={1}",
                        rowNames.Fields.Length,
                        rowTypes.Fields.Length));
            }

            string[] defaultValues = new string[rowTypes.Fields.Length];
            for (int i = 0; i < rowNames.Fields.Length; ++i)
            {
                string name = rowNames.Fields[i];
                string strType = rowTypes.Fields[i];
                DataTableType result = (from ddt in DataTableType.Types where ddt.Name == strType select ddt).FirstOrDefault();
                defaultValues[i] = result.Default;
                if (result != null)
                {
                    DataColumn dc = new DataColumn(name, result.Type);

                    dt.Columns.Add(dc);
                }
                else
                {
                    throw new Exception("無效的型別:" + strType);
                }
            }

            int rowCount = rowNames.Fields.Length;
            while (iterator.MoveNext())
            {
                CSV.Row row = iterator.Current;
                string[] fields = row.Fields;

                if (fields.Length == rowCount)
                {
                    DataRow dtRow = dt.NewRow();
                    int i = 0;
                    foreach (string name in rowNames.Fields)
                    {
                        string field = fields[i] == string.Empty
                                        ? defaultValues[i]
                                        : fields[i];
                        dtRow[name] = field;
                        ++i;
                    }

                    dt.Rows.Add(dtRow);
                }
                else
                {
                    throw new Exception(
                        string.Format(
                            "資料欄位與名稱數量不符: Index={0},RowCount={1},FieldCount={2}",
                            row.Index,
                            rowCount,
                            fields.Length));
                }
            }
        }
    }
}
