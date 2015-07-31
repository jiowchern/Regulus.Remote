// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RangeChancesTable.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the RangeChancesTable type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.Linq;

#endregion

namespace Regulus.Game
{
	public class RangeChancesTable<T> : ChancesTable<T>
	{
		public RangeChancesTable(Data[] datas) : base(datas)
		{
		}

		public override T Dice(float chances)
		{
			var minGate = 0.0f;
			foreach (var data in this._Datas)
			{
				if (minGate < chances && chances <= data.Value)
				{
					return data.Key;
				}

				minGate = data.Value;
			}

			return this._Datas.First().Key;
		}
	}
}