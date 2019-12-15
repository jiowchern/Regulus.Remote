using System.Collections.Generic;

namespace Regulus.Extension
{
	public static class Dictionary
	{
		public static IEnumerator<Value> Values<Key, Value>(this Dictionary<Key, Value> datas)
		{
			foreach(var data in datas)
			{
				yield return data.Value;
			}
		}

		public static IEnumerator<Key> Keys<Key, Value>(this Dictionary<Key, Value> datas)
		{
			foreach(var data in datas)
			{
				yield return data.Key;
			}
		}
	}
}
