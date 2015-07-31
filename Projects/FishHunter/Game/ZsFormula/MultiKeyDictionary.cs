// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiKeyDictionary.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the MultiKeyDictionary type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace VGame.Project.FishHunter.ZsFormula
{
	public class MultiKeyDictionary<TK1, TK2, TV> : Dictionary<TK1, Dictionary<TK2, TV>>
	{
		public TV this[TK1 key1, TK2 key2]
		{
			get
			{
				if (!ContainsKey(key1) || !this[key1].ContainsKey(key2))
				{
					throw new ArgumentOutOfRangeException();
				}

				return base[key1][key2];
			}

			set
			{
				if (!ContainsKey(key1))
				{
					this[key1] = new Dictionary<TK2, TV>();
				}

				this[key1][key2] = value;
			}
		}

		public new IEnumerable<TV> Values
		{
			get
			{
				return from baseDict in base.Values
				       from baseKey in baseDict.Keys
				       select baseDict[baseKey];
			}
		}

		public void Add(TK1 key1, TK2 key2, TV value)
		{
			if (!ContainsKey(key1))
			{
				this[key1] = new Dictionary<TK2, TV>();
			}

			this[key1][key2] = value;
		}

		public bool ContainsKey(TK1 key1, TK2 key2)
		{
			return ContainsKey(key1) && this[key1].ContainsKey(key2);
		}

		public Dictionary<TK2, TV>.ValueCollection FindValueByKey1(TK1 key1)
		{
			return this[key1].Values;
		}
	}

	public class MultiKeyDictionary<K1, K2, K3, V> : Dictionary<K1, MultiKeyDictionary<K2, K3, V>>
	{
		public V this[K1 key1, K2 key2, K3 key3]
		{
			get
			{
				return ContainsKey(key1)
					? this[key1][key2, key3]
					: default(V);
			}

			set
			{
				if (!ContainsKey(key1))
				{
					this[key1] = new MultiKeyDictionary<K2, K3, V>();
				}

				this[key1][key2, key3] = value;
			}
		}

		public bool ContainsKey(K1 key1, K2 key2, K3 key3)
		{
			return ContainsKey(key1) && this[key1].ContainsKey(key2, key3);
		}
	}
}