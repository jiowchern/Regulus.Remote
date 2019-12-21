using System.Linq;

namespace Regulus.Game
{
	public class RangeChancesTable<T> : ChancesTable<T>
	{
		public RangeChancesTable(Data[] datas) : base(datas)
		{
		}

		protected RangeChancesTable()
		{
		}

		public override T Dice(float chances)
		{
			var minGate = 0.0f;
			foreach(var data in _Datas)
			{
				if(minGate < chances && chances <= data.Value)
				{
					return data.Key;
				}

				minGate = data.Value;
			}

			return _Datas.First().Key;
		}
	}
}
