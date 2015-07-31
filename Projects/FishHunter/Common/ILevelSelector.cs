// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILevelSelector.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the ILevelSelector type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Remoting;

#endregion

namespace VGame.Project.FishHunter.Common
{
	public interface ILevelSelector
	{
		Value<int[]> QueryStages();

		Value<bool> Select(int level);
	}
}