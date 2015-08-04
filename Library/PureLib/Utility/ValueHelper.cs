// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueHelper.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the ValueHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace Regulus.Utility
{
	public class ValueHelper
	{
		public IEnumerator<T> GetEnumItems<T>() where T : class
		{
			foreach (var item in Enum.GetValues(typeof (T)))
			{
				yield return item as T;
			}
		}

		public static T DeepCopy<T>(T obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("Object cannot be null");
			}

			return (T)ValueHelper._CopyProcess(obj);
		}

		public static object DeepCopy(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("Object cannot be null");
			}

			return ValueHelper._CopyProcess(obj);
		}

		private static object _CopyProcess(object obj)
		{
			if (obj == null)
			{
				return null;
			}

			var type = obj.GetType();
			if (type.IsValueType || type == typeof (string))
			{
				return obj;
			}

			if (type.IsArray)
			{
				var elementType = Type.GetType(
					type.FullName.Replace("[]", string.Empty));
				var array = obj as Array;
				var copied = Array.CreateInstance(elementType, array.Length);
				for (var i = 0; i < array.Length; i++)
				{
					copied.SetValue(ValueHelper._CopyProcess(array.GetValue(i)), i);
				}

				return Convert.ChangeType(copied, obj.GetType());
			}

			if (type.IsClass)
			{
				var toret = Activator.CreateInstance(obj.GetType());
				var fields = type.GetFields(BindingFlags.Public |
				                            BindingFlags.NonPublic | BindingFlags.Instance);
				foreach (var field in fields)
				{
					var fieldValue = field.GetValue(obj);
					if (fieldValue == null)
					{
						continue;
					}

					field.SetValue(toret, ValueHelper._CopyProcess(fieldValue));
				}

				return toret;
			}

			throw new ArgumentException("Unknown type");
		}

		public static bool DeepEqual(object obj1, object obj2)
		{
			return ValueHelper._EqualProcess(obj1, obj2);
		}

		private static bool _EqualProcess(object obj, object obj1)
		{
			if (obj == null && obj1 == null)
			{
				return true;
			}

			if (obj != null && obj1 == null)
			{
				return false;
			}

			if (obj == null && obj1 != null)
			{
				return false;
			}

			if (obj1.GetType() != obj.GetType())
			{
				return false;
			}

			var type = obj.GetType();
			if (type.IsValueType || type == typeof (string))
			{
				return obj.Equals(obj1);
			}

			if (type.IsArray)
			{
				var elementType = Type.GetType(
					type.FullName.Replace("[]", string.Empty));
				var array = obj as Array;

				// Array copied = Array.CreateInstance(elementType, array.Length);
				var copied = obj1 as Array;
				for (var i = 0; i < array.Length; i++)
				{
					if (ValueHelper._EqualProcess(array.GetValue(i), copied.GetValue(i)) == false)
					{
						return false;
					}
				}

				return true;
			}

			if (type.IsClass)
			{
				var fields = type.GetFields(BindingFlags.Public |
				                            BindingFlags.NonPublic | BindingFlags.Instance);
				foreach (var field in fields)
				{
					var fieldValue = field.GetValue(obj);
					if (ValueHelper._EqualProcess(field.GetValue(obj), field.GetValue(obj1)) == false)
					{
						return false;
					}
				}

				return true;
			}

			throw new ArgumentException("無法比較的類型.");
		}
	}
}