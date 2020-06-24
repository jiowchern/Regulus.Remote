using System;
using System.Linq;

namespace Regulus.Utility
{
	public class ChancesTable<TTarget>
	{
		public class Item
		{
			public TTarget Target { get; set; }

			public int Scale { get; set; }

			public Item()
			{
			}

			public Item(TTarget target, int scale)
			{
				Target = target;
				Scale = scale;
			}
		}

		protected Item[] _Datas;

		public ChancesTable(System.Collections.Generic.IEnumerable<Item> items)
		{
			
			_Datas = items.ToArray();
			if (_Datas.Length == 0)
				throw new ArgumentException("Item need to be greater than 0.");
		}		

		public  TTarget Get(int chances)
		{
			var minGate = 0;
			foreach(var data in _Datas)
			{
				var gate = minGate + data.Scale + 1;

				if(minGate <= chances && chances < gate )
				{
					return data.Target;
				}

				minGate += data.Scale;
			}

			throw new InvalidOperationException($"Value must be between 1 and 0");
			
		}
	}
}
