// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScoreOddsTable.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the ScoreOddsTable type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Game;

#endregion

namespace VGame.Project.FishHunter.ZsFormula.Data
{
	internal class ScoreOddsTable : ChancesTable<int>
	{
		public ScoreOddsTable(Data[] datas)
			: base(datas)
		{
		}
	}
}