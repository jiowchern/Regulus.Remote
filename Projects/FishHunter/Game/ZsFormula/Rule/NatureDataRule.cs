#region Test_Region

using System.Collections.Generic;
using System.Linq;


using Regulus.Game;


using VGame.Project.FishHunter.ZsFormula.Data;

#endregion

namespace VGame.Project.FishHunter.ZsFormula.Rule
{
	public class NatureDataRule
	{
		public NatureDataRule()
		{
		}

		public int Run(long buffer_value, int base_value)
		{
			var natureValue = 0;

			var datas = new NatureBufferChancesTable().Get().ToDictionary(x => x.Key);
			
			foreach (var data in datas)
			{
				if (buffer_value > (data.Key * base_value))
				{
					natureValue = (int)data.Value.Value;
				}
			}

			return natureValue;
		}
	}
}