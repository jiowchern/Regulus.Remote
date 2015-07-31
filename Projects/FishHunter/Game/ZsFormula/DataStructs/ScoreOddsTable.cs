// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScoreOddsTable.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the ScoreOddsTable type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using Regulus.Game;

namespace VGame.Project.FishHunter.ZsFormula.DataStructs
{
	internal class ScoreOddsTable : ChancesTable<int>
	{
		public ScoreOddsTable(Data[] datas)
			: base(datas)
		{
		}
	}
}