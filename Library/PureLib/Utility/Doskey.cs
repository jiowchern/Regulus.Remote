// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Doskey.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Doskey type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.Collections.Generic;

#endregion

namespace Regulus.Utility
{
	public class Doskey
	{
		private readonly int _Capacity;

		private readonly List<string> _Stack;

		private int _Current;

		public Doskey(int capacity)
		{
			this._Capacity = capacity;
			this._Stack = new List<string>(capacity);
		}

		public void Record(string p)
		{
			this._Stack.Add(p);

			if (this._Stack.Count > this._Capacity)
			{
				this._Stack.RemoveAt(0);
			}

			this._Current = this._Stack.Count;
		}

		public string TryGetPrev()
		{
			if (this._Stack.Count <= 0)
			{
				return null;
			}

			if (this._Current - 1 >= 0)
			{
				return this._Stack[--this._Current];
			}

			return null;
		}

		public string TryGetNext()
		{
			if (this._Stack.Count <= 0)
			{
				return null;
			}

			if (this._Current + 1 < this._Stack.Count)
			{
				return this._Stack[++this._Current];
			}

			return null;
		}
	}
}