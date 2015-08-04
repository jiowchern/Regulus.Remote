// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NatureDataRule.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the NatureDataRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using VGame.Project.FishHunter.ZsFormula.DataStructs;

#endregion

namespace VGame.Project.FishHunter.ZsFormula.Rules
{
	public class NatureDataRule
	{
		private readonly NatureBufferChancesTable _NatureBufferChancesTable;

		public NatureDataRule()
		{
			_NatureBufferChancesTable = new NatureBufferChancesTable(null);
		}

		public int Run(long buffer_value, int base_value)
		{
			var natureValue = 0;

			foreach (var data in _NatureBufferChancesTable.Datas)
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