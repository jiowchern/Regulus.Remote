using System;


using Regulus.Remoting;


using VGame.Project.FishHunter.Common.Data;

namespace VGame.Project.FishHunter.Common.GPI
{
	public interface IFormulaPlayerRecorder
	{
		/// <summary>
		/// 查詢算法伺服器的玩家歷程
		/// </summary>
		/// <param name="player_id"></param>
		/// <returns></returns>
		Value<FormulaPlayerRecord> Query(Guid player_id);

		/// <summary>
		/// 儲存算法伺服器的玩家歷程
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		Value<bool> Save(FormulaPlayerRecord record);
	}
}