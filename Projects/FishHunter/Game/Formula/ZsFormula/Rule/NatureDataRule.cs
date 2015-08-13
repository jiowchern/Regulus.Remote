// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NatureDataRule.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the NatureDataRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;


using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule
{
	/// <summary>
	/// 計算自然buffer的規則
	/// </summary>
	public class NatureDataRule
	{
		public int Run(long buffer_value, int base_value)
		{
			var natureValue = 0;

			var datas = new NatureBufferChancesTable().Get().ToDictionary(x => x.Key);

			foreach(var data in datas)
			{
				if(buffer_value > (data.Key * base_value))
				{
					natureValue = (int)data.Value.Value;
				}
			}

			return natureValue;
		}
	}
}
