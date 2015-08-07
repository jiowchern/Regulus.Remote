// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChancesTable.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the ChancesTable type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Linq;

#endregion

namespace Regulus.Game
{
	public class ChancesTable<T>
	{
		protected Data[] _Datas;

		protected ChancesTable(Data[] datas)
		{
			if (datas.Length == 0)
			{
				throw new ArgumentException("參數數量不可為0");
			}

			this._Datas = datas;
		}

		protected ChancesTable()
		{
			
		}


		public class Data
		{
			public Data()
			{
			}

			public Data(T key, float value)
			{
				Key = key;
				Value = value;
			}

			public T Key { get; set; }

			public float Value { get; set; }
		}

		public virtual T Dice(float chances)
		{
			var minGate = 0.0f;
			foreach (var data in this._Datas)
			{
				var gate = minGate + data.Value;

				if (minGate <= chances && chances < gate)
				{
					return data.Key;
				}

				minGate += data.Value;
			}

			return this._Datas.First().Key;
		}
	}
}