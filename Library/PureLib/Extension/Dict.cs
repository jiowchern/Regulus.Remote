// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Dict.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Dictionary type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.Collections.Generic;

#endregion

namespace Regulus.Extension
{
	public static class Dictionary
	{
		public static IEnumerator<Value> Values<Key, Value>(this Dictionary<Key, Value> datas)
		{
			foreach (var data in datas)
			{
				yield return data.Value;
			}
		}

		public static IEnumerator<Key> Keys<Key, Value>(this Dictionary<Key, Value> datas)
		{
			foreach (var data in datas)
			{
				yield return data.Key;
			}
		}
	}
}