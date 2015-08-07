// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFishStageQueryer.cs" company="">
//   
// </copyright>
// <summary>
//   魚場請求
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using Regulus.Remoting;

namespace VGame.Project.FishHunter.Common.GPI
{
	/// <summary>
	///     魚場請求
	/// </summary>
	public interface IFishStageQueryer
	{
		/// <summary>
		/// 查詢請求
		/// </summary>
		/// <param name="player_id">
		/// </param>
		/// <param name="fish_stage">
		/// </param>
		/// <returns>
		/// 魚場
		/// </returns>
		Value<IFishStage> Query(long player_id, int fish_stage);
	}

}