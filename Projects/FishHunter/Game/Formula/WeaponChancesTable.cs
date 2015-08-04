// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeaponChancesTable.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the WeaponChancesTable type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Game;

#endregion

namespace VGame.Project.FishHunter
{
	internal class WeaponChancesTable : ChancesTable<int>
	{
		public WeaponChancesTable(Data[] datas)
			: base(datas)
		{
		}
	}
}