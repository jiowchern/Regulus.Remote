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

			_StageDataVisit.NowUseData.Recode.WinTimes += 1;
			_StageDataVisit.NowUseData.Recode.WinTotal += win;

			player_data.Recode.WinTimes += 1;
			player_data.Recode.WinTotal += win;

			// 玩家阶段起伏的调整
			if (player_data.Status <= 0)
			{
				return;
			}

			player_data.BufferValue -= win;
			player_data.Recode.AsnWin += win;
		}
	}
}