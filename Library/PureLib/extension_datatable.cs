using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Extension
{



    public static class System_Data_DataTable
    {
        public class DataTableType
        {
            public DataTableType(string name, Type type ,string def_value)
            {
                Name = name;
                Type = type;
                Default = def_value;
            }
            public string Name { get; private set; }
            public Type Type { get; private set; }
            public string Default { get; private set; }
            public static readonly DataTableType[] Types = new DataTableType[] {    new DataTableType("int" , typeof(int) , "0") ,
                                                                                    new DataTableType("float" , typeof(float) , "0.0"),
                                                                                    new DataTableType("string" , typeof(string) , "")};
        }

        public static void ReadCSV(this System.Data.DataTable dt, string path, string token)
        {


            var iterator = Regulus.Utility.CSV.Read(path, token).GetEnumerator();

            //System.Data.DataTable dt = new System.Data.DataTable();
			// 第一行
			//iterator.MoveNext();
            // 第二行
			iterator.MoveNext();
            Regulus.Utility.CSV.Row rowNames = iterator.Current;
			// 第三行
            iterator.MoveNext();
            Regulus.Utility.CSV.Row rowTypes = iterator.Current;

            if (rowTypes.Fields.Length != rowNames.Fields.Length)
            {
				throw new System.Exception(string.Format("名稱與型別數量不符: RowNameCount={0},RowTypeCount={1}", rowNames.Fields.Length, rowTypes.Fields.Length));
            }

            string[] defaultValues = new string[rowTypes.Fields.Length];
            for (int i = 0; i < rowNames.Fields.Length; ++i)
            {
                string name = rowNames.Fields[i];
                string strType = rowTypes.Fields[i];
                var result = (from ddt in DataTableType.Types where ddt.Name == strType select ddt).FirstOrDefault();
                defaultValues[i] = result.Default;
                if (result != null)
                {
                    System.Data.DataColumn dc = new System.Data.DataColumn(name, result.Type);
                    
                    dt.Columns.Add(dc);
                }
                else
                {
                    throw new System.Exception("無效的型別:" + strType);
                }
            }

            int rowCount = rowNames.Fields.Length;
            while (iterator.MoveNext() == true)
            {
                var row = iterator.Current;
                string[] fields = row.Fields;

				if (fields.Length == rowCount)
                {
                    System.Data.DataRow dtRow = dt.NewRow();
					int i = 0;
                    foreach (var name in rowNames.Fields)
                    {
                        var field = fields[i] == "" ? defaultValues[i] : fields[i] ;
                        dtRow[name] = field;
                        ++i;
                    }
                    dt.Rows.Add(dtRow);
                }
                else
                {
					throw new System.Exception(string.Format("資料欄位與名稱數量不符: Index={0},RowCount={1},FieldCount={2}", row.Index, rowCount, fields.Length));
                }
            }

        }
    }

}