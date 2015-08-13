using System;
using System.Collections.Generic;

namespace Regulus.Extension
{
	public static class ShowMember
	{
		public static string ShowMembers<T>(this T obj)
		{
			return ShowMember._ShowMembers(obj, ", ");
		}

		public static string ShowMembers<T>(this T obj, string join_token)
		{
			return ShowMember._ShowMembers(obj, join_token);
		}

		private static string _ShowMembers<T>(T obj, string join_token)
		{
			var type = typeof(T);
			var fields = type.GetFields();
			var properties = type.GetProperties();
			var user = obj;

			var values = new List<string>();

			Array.ForEach(fields, field => values.Add(field.Name + " : " + field.GetValue(user)));
			Array.ForEach(
				properties, 
				property =>
				{
					if(property.CanRead)
					{
						values.Add(property.Name + " : " + property.GetValue(user, null));
					}
				});

			return string.Join(join_token, values.ToArray());
		}
	}
}
