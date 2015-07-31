// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFishStageQueryer.cs" company="">
//   
// </copyright>
// <summary>
//   魚場請求
// </summary>
// --------------------------------------------------------------------------------------------------------------------




using System;

using Regulus.Remoting;

namespace VGame.Project.FishHunter.Common
{
	/// <summary>
	///     魚場請求
	/// </summary>
	public interface IFishStageQueryer
	{
		/// <summary>
		///     查詢請求
		/// </summary>
		/// <param name="玩家id"></param>
		/// <param name="魚場id"></param>
		/// <returns>魚場</returns>
		Value<IFishStage> Query(long player_id, byte fish_stage);
	}

}