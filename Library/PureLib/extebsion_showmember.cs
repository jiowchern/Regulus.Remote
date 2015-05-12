using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Regulus.Extension
{
    public static class extebsion_showmember
    {
        public static string ShowMembers<T>(this T obj)
        {            
            return _ShowMembers(obj , ", ");
        }

        public static string ShowMembers<T>(this T obj , string join_token)
        {
            return _ShowMembers(obj, join_token);
        }


        static string _ShowMembers<T>(T obj , string join_token)
        {
            Type type = typeof(T);
            FieldInfo[] fields = type.GetFields();
            PropertyInfo[] properties = type.GetProperties();
            T user = obj;

            List<string> values = new List<string>();

            Array.ForEach(fields, (field) => values.Add(field.Name + " : " + field.GetValue(user)));
            Array.ForEach(properties, (property) =>
            {
                if (property.CanRead)
                    values.Add(property.Name + " : " + property.GetValue(user, null));
            });

            return String.Join(join_token, values.ToArray());
        }
    }
}
