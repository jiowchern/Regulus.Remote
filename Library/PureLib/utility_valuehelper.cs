using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Regulus.Utility
{
    public class ValueHelper
    {
        public static T DeepCopy<T>(T obj)
        {
            if (obj == null)
                throw new ArgumentNullException("Object cannot be null");
            return (T)_CopyProcess(obj);
        }

        public static object DeepCopy(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("Object cannot be null");
            return _CopyProcess(obj);
        }

        static object _CopyProcess(object obj)
        {
            if (obj == null)
                return null;
            Type type = obj.GetType();
            if (type.IsValueType || type == typeof(string))
            {
                return obj;
            }
            else if (type.IsArray)
            {
                Type elementType = Type.GetType(
                     type.FullName.Replace("[]", string.Empty));
                var array = obj as Array;
                Array copied = Array.CreateInstance(elementType, array.Length);
                for (int i = 0; i < array.Length; i++)
                {
                    copied.SetValue(_CopyProcess(array.GetValue(i)), i);
                }
                return Convert.ChangeType(copied, obj.GetType());
            }
            else if (type.IsClass)
            {
                object toret = Activator.CreateInstance(obj.GetType());
                FieldInfo[] fields = type.GetFields(BindingFlags.Public |
                            BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    object fieldValue = field.GetValue(obj);
                    if (fieldValue == null)
                        continue;
                    field.SetValue(toret, _CopyProcess(fieldValue));
                }
                return toret;
            }
            else
                throw new ArgumentException("Unknown type");
        }

        public static bool DeepEqual(object obj1 , object obj2)
        {           
            return _EqualProcess(obj1,obj2);
        }

        static bool _EqualProcess(object obj, object obj1)
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
                return false;


            Type type = obj.GetType();
            if (type.IsValueType || type == typeof(string))
            {
                return obj.Equals(obj1);
            }
            else if (type.IsArray)
            {
                Type elementType = Type.GetType(
                     type.FullName.Replace("[]", string.Empty));
                var array = obj as Array;
                Array copied = Array.CreateInstance(elementType, array.Length);
                for (int i = 0; i < array.Length; i++)
                {
                    if (_EqualProcess(array.GetValue(i), copied.GetValue(i)) == false)
                    {
                        return false;
                    }                    
                }
                return true;
            }
            else if (type.IsClass)
            {
                
                FieldInfo[] fields = type.GetFields(BindingFlags.Public |
                            BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    object fieldValue = field.GetValue(obj);
                    if (_EqualProcess(field.GetValue(obj), field.GetValue(obj1)) == false)
                        return false;                    
                }
                return true;
            }
            else
                throw new ArgumentException("無法比較的類型.");
        }

    }
}
