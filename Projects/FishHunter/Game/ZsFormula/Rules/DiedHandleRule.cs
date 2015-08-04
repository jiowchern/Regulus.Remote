// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiedHandleRule.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the DiedHandleRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;

using VGame.Project.FishHunter.ZsFormula.DataStructs;

#endregion


namespace VGame.Project.FishHunter.ZsFormula.Rules
{
	/// <summary>
	/// 分數記錄
	/// </summary>
	public class DiedHandleRule
	{
		private readonly StageDataVisit _StageDataVisit;

		public DiedHandleRule(StageDataVisit stage_data_visit)
		{
			_StageDataVisit = stage_data_visit;
		}

		/// <summary>
		/// </summary>
		/// <param name="win_func"></param>
		/// <param name="player_data"></param>
		public void Run(Func<int> win_func, Player.Data player_data)
		{
			if (win_func == null)
			{
				return;
			}

			var win = win_func();

			var bufferData = _StageDataVisit.FindBufferData(
				_StageDataVisit.NowUseBlock, 
				StageDataTable.BufferData.BUFFER_TYPE.NORMAL);

			bufferData.Buffer -= win;

			_StageDataVisit.NowUseData.RecodeData.WinFrequency += 1;
			_StageDataVisit.NowUseData.RecodeData.WinScore += win;

			player_data.RecodeData.WinFrequency += 1;
			player_data.RecodeData.WinScore += win;

			// 玩家阶段起伏的调整
			if (player_data.Status <= 0)
			{
				return;
			}

			player_data.BufferValue -= win;
			player_data.RecodeData.AsnWin += win;
		}
	}
}