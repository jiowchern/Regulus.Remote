using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Game
{
    namespace Data
    {
        using Regulus.Extension;
        [AttributeUsage(AttributeTargets.Class)]
        public class TableAttribute : Attribute
        {
            public TableAttribute(string name) { Name = name; }
            public string Name { get; private set; }
        }

        [AttributeUsage(AttributeTargets.Property)]
        public class FieldAttribute : Attribute
        {
            public FieldAttribute(string name = "") { Name = name; }
            public string Name { get; private set; }
        }

        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
        public class BlockFieldAttribute : Attribute
        {
            public BlockFieldAttribute(string name, string[] fields) { Name = name; Fields = fields; }
            public string Name { get; private set; }
            public string[] Fields { get; private set; }

        }

        [AttributeUsage(AttributeTargets.Property)]
        public class ArrayFieldAttribute : Attribute
        {
            public ArrayFieldAttribute(string[] fields) { Fields = fields; }
            public string[] Fields { get; private set; }
        }
        [AttributeUsage(AttributeTargets.Property)]
        public class ReferenceFieldAttribute : Attribute
        {
            public ReferenceFieldAttribute(string name, string key_field) { Name = name; Key = key_field; }
            public string Name { get; private set; }
            public string Key { get; private set; }
        }

        public class PrototypeFactory : IDisposable
        {
            System.Data.DataSet _Datas = new System.Data.DataSet("PrototypeFactory");
            public void LoadCSV(string name, string path , string ext_name = "")
            {
				System.Data.DataTable dt = new System.Data.DataTable(name + ext_name);
                dt.ReadCSV(path, "\t");
				if (_Datas.Tables.Contains(name + ext_name) == true)
				{
					_Datas.Tables.Remove(name + ext_name);					
				}
				_Datas.Tables.Add(dt);
            }

			private Array _GeneratePrototype(Type prototype_type, Func<System.Data.DataRow, bool> filter, string ext_name)
            {
                var tableInfo = prototype_type.GetCustomAttributes(typeof(Regulus.Game.Data.TableAttribute), false).FirstOrDefault() as Regulus.Game.Data.TableAttribute;
                if (tableInfo != null)
                {

					if (_Datas.Tables.Contains(tableInfo.Name + ext_name) == true)
                    {
						System.Data.DataTable table = _Datas.Tables[tableInfo.Name + ext_name];

                        var rows = table.Rows.Cast<System.Data.DataRow>().Where(filter);
                        int prototypeIdx = 0;

                        Array prototypes = Array.CreateInstance(prototype_type, rows.Count());
                        var propertys = prototype_type.GetProperties();
                        #region
                        foreach (var row in rows)
                        {
                            System.Data.DataRow dr = (System.Data.DataRow)row;
                            var prototype = Activator.CreateInstance(prototype_type);
                            prototypes.SetValue(prototype, prototypeIdx++);
                            foreach (var property in propertys)
                            {
                                #region
                                var fieldInfo = property.GetCustomAttributes(typeof(Regulus.Game.Data.FieldAttribute), false).FirstOrDefault() as Regulus.Game.Data.FieldAttribute;
                                if (fieldInfo != null)
                                {
                                    string fieldName = fieldInfo.Name;
                                    if (fieldName == "")
                                    {
                                        fieldName = property.Name;
                                    }
                                    
                                    property.SetValue(prototype, dr[fieldName], null);
                                }
                                #endregion

                                #region
                                var refFieldInfo = property.GetCustomAttributes(typeof(Regulus.Game.Data.ReferenceFieldAttribute), false).FirstOrDefault() as Regulus.Game.Data.ReferenceFieldAttribute;
                                if (refFieldInfo != null)
                                {

                                    var refFieldType = property.PropertyType.GetElementType();
                                    string key1Name = refFieldInfo.Name;

                                    Array fields = _GeneratePrototype(refFieldType, (data_row) =>
                                    {
                                        var k1 = data_row[refFieldInfo.Key].ToString();
                                        var k2 = dr[key1Name].ToString();
                                        return k1 == k2;


                                    },"");
                                    
                                    property.SetValue(prototype, fields, null);


                                }
                                #endregion

                                #region
                                var arrayFieldInfo = property.GetCustomAttributes(typeof(Regulus.Game.Data.ArrayFieldAttribute), false).FirstOrDefault() as Regulus.Game.Data.ArrayFieldAttribute;
                                if (arrayFieldInfo != null)
                                {
                                    Array fields = Array.CreateInstance(property.PropertyType.GetElementType(), arrayFieldInfo.Fields.Length);
                                    int objIdx = 0;
                                    foreach (var field in arrayFieldInfo.Fields)
                                    {
                                        var col = table.Columns[field];
                                        Type colType = col.DataType;
                                        fields.SetValue(dr[field], objIdx++);

                                    }
                                    property.SetValue(prototype, fields, null);
                                }
                                #endregion


                                #region
                                var blockFieldInfos = property.GetCustomAttributes(typeof(Regulus.Game.Data.BlockFieldAttribute), false) as Regulus.Game.Data.BlockFieldAttribute[];

                                if (blockFieldInfos.Length > 0)
                                {
                                    var refFieldType = property.PropertyType.GetElementType();
                                    var eleCount = 0;
                                    foreach (var bfi in blockFieldInfos)
                                    {
                                        if (bfi.Fields.Length > eleCount)
                                            eleCount = bfi.Fields.Length;
                                    }

                                    Array fields = Array.CreateInstance(refFieldType, eleCount);


                                    

                                    object[] eles = new object[eleCount];
                                    for (int i = 0; i < eleCount; ++i)
                                    {
                                        eles[i] = Activator.CreateInstance(refFieldType);
                                        fields.SetValue(eles[i], i);
                                    }



                                    foreach (var blockFieldInfo in blockFieldInfos)
                                    {
                                        Regulus.Game.Data.BlockFieldAttribute bfa = blockFieldInfo as Regulus.Game.Data.BlockFieldAttribute;
                                        var refProperty = refFieldType.GetProperty(bfa.Name);

                                        if (refProperty != null)
                                        {
                                            int fieldIdx = 0;
                                            foreach (var rowName in bfa.Fields)
                                            {

                                                refProperty.SetValue(eles[fieldIdx], dr[rowName], null);
                                                fieldIdx++;
                                            }
                                        }

                                    }

                                    property.SetValue(prototype, fields, null);
                                }
                                #endregion
                            }
                        }
                        #endregion

                        return prototypes;
                    }
                }
                return null;
            }
            public T[] GeneratePrototype<T>(string ext_name ="" ) where T : new()
            {
				Array prototypes = _GeneratePrototype(typeof(T), (row) => { return true; }, ext_name);
                if (prototypes != null)
                {
                    return prototypes.Cast<T>().ToArray<T>();
                }
                return null;

            }

            void IDisposable.Dispose()
            {
                _Datas.Dispose();
            }
        }
    }

}