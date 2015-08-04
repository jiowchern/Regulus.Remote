// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumDescription.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the EnumDescriptionAttribute type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Regulus.Utility
{
	[AttributeUsage(AttributeTargets.Field)]
	public class EnumDescriptionAttribute : Attribute
	{
		public string Message { get; private set; }

		public EnumDescriptionAttribute(string message)
		{
			this.Message = message;
		}
	}

	public static class EnumDescriptionExtension
	{
		public static string GetEnumDescription<T>(this T enum_instance)
		{
			var memberInfo = typeof (T).GetMember(enum_instance.ToString())
				.FirstOrDefault();
			if (memberInfo != null)
			{
				var attribute =
					memberInfo.GetCustomAttributes(typeof (EnumDescriptionAttribute), false).FirstOrDefault() as
						EnumDescriptionAttribute;
				if (attribute != null)
				{
					return attribute.Message;
				}

				return string.Empty;
			}

			return null;
		}
	}

	public static class EnumHelper
	{
		public static IEnumerable<T> GetEnums<T>()
		{
			return Enum.GetValues(typeof (T)).Cast<T>();
		}

		public static IEnumerable<Enum> GetFlags(this Enum enum_instance)
		{
			var ienum = Convert.ToUInt64(enum_instance);
			var availableSuits = Enum.GetValues(enum_instance.GetType()).Cast<Enum>();
			foreach (var suit in availableSuits)
			{
				var iflag = Convert.ToUInt64(suit);
				if ((ienum & iflag) > 0)
				{
					yield return suit;
				}
			}
		}

		public static IEnumerable<bool> ToFlags(this Enum enum_instance)
		{
			var ienum = Convert.ToUInt64(enum_instance);
			var availableSuits = Enum.GetValues(enum_instance.GetType()).Cast<Enum>();
			foreach (var suit in availableSuits)
			{
				var iflag = Convert.ToUInt64(suit);
				var result = ienum & iflag;
				yield return result > 0;
			}
		}
	}
}