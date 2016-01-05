using System.Linq;

using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule.Calculation
{
	/// <summary>
	///     計算自然buffer的規則
	/// </summary>
	public class NatureData
	{
		public int Run(long buffer_value, int base_value)
		{
			var natureValue = 0;

			var datas = new NatureBufferChancesTable().Get()
													.ToDictionary(x => x.Key);

			// 沒有的話要回傳最小值
			if(!datas.Any(data => buffer_value > (data.Key * base_value)))
			{
				return (int)datas.First()
								.Value.Value;
			}

			// 比對最大值
			foreach(var data in datas.Where(data => buffer_value > (data.Key * base_value)))
			{
				natureValue = (int)data.Value.Value;
			}

			return natureValue;
		}
	}
}
