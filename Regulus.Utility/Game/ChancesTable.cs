using System;
using System.Linq;

namespace Regulus.Game
{
	public class ChancesTable<T>
	{
		public class Data
		{
			public T Key { get; set; }

			public float Value { get; set; }

			public Data()
			{
			}

			public Data(T key, float value)
			{
				Key = key;
				Value = value;
			}
		}

		protected Data[] _Datas;

		protected ChancesTable(Data[] datas)
		{
			if(datas.Length == 0)
			{
				throw new ArgumentException("參數數量不可為0");
			}

			_Datas = datas;
		}

		protected ChancesTable()
		{
		}

		public virtual T Dice(float chances)
		{
			var minGate = 0.0f;
			foreach(var data in _Datas)
			{
				var gate = minGate + data.Value;

				if(minGate <= chances && chances < gate)
				{
					return data.Key;
				}

				minGate += data.Value;
			}

			return _Datas.First().Key;
		}
	}
}
