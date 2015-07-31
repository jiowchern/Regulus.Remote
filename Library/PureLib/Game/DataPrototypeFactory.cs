// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataPrototypeFactory.cs" company="">
//   
// </copyright>
// <summary>
//   DataPrototypeFactory.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Data;
using System.Linq;

using Regulus.Extension;

#endregion

namespace Regulus.Game
{
	namespace Data
	{
		[AttributeUsage(AttributeTargets.Class)]
		public class TableAttribute : Attribute
		{
			public string Name { get; private set; }

			public TableAttribute(string name)
			{
				this.Name = name;
			}
		}

		[AttributeUsage(AttributeTargets.Property)]
		public class FieldAttribute : Attribute
		{
			public string Name { get; private set; }

			public FieldAttribute(string name = "")
			{
				this.Name = name;
			}
		}

		[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
		public class BlockFieldAttribute : Attribute
		{
			public string Name { get; private set; }

			public string[] Fields { get; private set; }

			public BlockFieldAttribute(string name, string[] fields)
			{
				this.Name = name;
				this.Fields = fields;
			}
		}

		[AttributeUsage(AttributeTargets.Property)]
		public class ArrayFieldAttribute : Attribute
		{
			public string[] Fields { get; private set; }

			public ArrayFieldAttribute(string[] fields)
			{
				this.Fields = fields;
			}
		}

		[AttributeUsage(AttributeTargets.Property)]
		public class ReferenceFieldAttribute : Attribute
		{
			public string Name { get; private set; }

			public string Key { get; private set; }

			public ReferenceFieldAttribute(string name, string key_field)
			{
				this.Name = name;
				this.Key = key_field;
			}
		}

		public class PrototypeFactory : IDisposable
		{
			private readonly DataSet _Datas = new DataSet("PrototypeFactory");

			void IDisposable.Dispose()
			{
				this._Datas.Dispose();
			}

			public void LoadCSV(string name, string path, string ext_name)
			{
				var dt = new DataTable(name + ext_name);
				dt.ReadCSV(path, "\t");
				if (this._Datas.Tables.Contains(name + ext_name))
				{
					this._Datas.Tables.Remove(name + ext_name);
				}

				this._Datas.Tables.Add(dt);
			}

			private Array _GeneratePrototype(Type prototype_type, Func<DataRow, bool> filter, string ext_name)
			{
				var tableInfo =
					prototype_type.GetCustomAttributes(typeof (TableAttribute), false).FirstOrDefault() as TableAttribute;
				if (tableInfo != null)
				{
					if (this._Datas.Tables.Contains(tableInfo.Name + ext_name))
					{
						var table = this._Datas.Tables[tableInfo.Name + ext_name];

						var rows = table.Rows.Cast<DataRow>().Where(filter);
						var prototypeIdx = 0;

						var prototypes = Array.CreateInstance(prototype_type, rows.Count());
						var propertys = prototype_type.GetProperties();

						#region

						foreach (var row in rows)
						{
							var dr = row;
							var prototype = Activator.CreateInstance(prototype_type);
							prototypes.SetValue(prototype, prototypeIdx++);
							foreach (var property in propertys)
							{
								#region

								var fieldInfo = property.GetCustomAttributes(typeof (FieldAttribute), false).FirstOrDefault() as FieldAttribute;
								if (fieldInfo != null)
								{
									var fieldName = fieldInfo.Name;
									if (fieldName == string.Empty)
									{
										fieldName = property.Name;
									}

									property.SetValue(prototype, dr[fieldName], null);
								}

								#endregion

								#region

								var refFieldInfo =
									property.GetCustomAttributes(typeof (ReferenceFieldAttribute), false).FirstOrDefault() as
										ReferenceFieldAttribute;
								if (refFieldInfo != null)
								{
									var refFieldType = property.PropertyType.GetElementType();
									var key1Name = refFieldInfo.Name;

									var fields = this._GeneratePrototype(refFieldType, data_row =>
									{
										var k1 = data_row[refFieldInfo.Key].ToString();
										var k2 = dr[key1Name].ToString();
										return k1 == k2;
									}, string.Empty);

									property.SetValue(prototype, fields, null);
								}

								#endregion

								#region

								var arrayFieldInfo =
									property.GetCustomAttributes(typeof (ArrayFieldAttribute), false).FirstOrDefault() as ArrayFieldAttribute;
								if (arrayFieldInfo != null)
								{
									var fields = Array.CreateInstance(property.PropertyType.GetElementType(), arrayFieldInfo.Fields.Length);
									var objIdx = 0;
									foreach (var field in arrayFieldInfo.Fields)
									{
										var col = table.Columns[field];
										var colType = col.DataType;
										fields.SetValue(dr[field], objIdx++);
									}

									property.SetValue(prototype, fields, null);
								}

								#endregion

								#region

								var blockFieldInfos = property.GetCustomAttributes(typeof (BlockFieldAttribute), false) as BlockFieldAttribute[];

								if (blockFieldInfos.Length > 0)
								{
									var refFieldType = property.PropertyType.GetElementType();
									var eleCount = 0;
									foreach (var bfi in blockFieldInfos)
									{
										if (bfi.Fields.Length > eleCount)
										{
											eleCount = bfi.Fields.Length;
										}
									}

									var fields = Array.CreateInstance(refFieldType, eleCount);

									var eles = new object[eleCount];
									for (var i = 0; i < eleCount; ++i)
									{
										eles[i] = Activator.CreateInstance(refFieldType);
										fields.SetValue(eles[i], i);
									}

									foreach (var blockFieldInfo in blockFieldInfos)
									{
										var bfa = blockFieldInfo;
										var refProperty = refFieldType.GetProperty(bfa.Name);

										if (refProperty != null)
										{
											var fieldIdx = 0;
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

			public T[] GeneratePrototype<T>(string ext_name = "") where T : new()
			{
				var prototypes = this._GeneratePrototype(typeof (T), row => { return true; }, ext_name);
				if (prototypes != null)
				{
					return prototypes.Cast<T>().ToArray<T>();
				}

				return null;
			}
		}
	}
}