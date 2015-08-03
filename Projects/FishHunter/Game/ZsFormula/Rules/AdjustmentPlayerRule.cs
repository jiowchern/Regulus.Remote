// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdjustmentPlayerRule.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   玩家阶段起伏的调整
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using Regulus.Utility;

using VGame.Project.FishHunter.ZsFormula.DataStructs;

namespace VGame.Project.FishHunter.ZsFormula.Rules
{
	/// <summary>
	///     玩家阶段起伏的调整
	/// </summary>
	public class AdjustmentPlayerRule
	{
		private readonly Player.Data _PlayerData;

		private readonly StageDataVisit _StageDataVisit;

		private static int RandNumber
		{
			get { return Random.Instance.NextInt(0, 1000) % 1000; }
		}

		public AdjustmentPlayerRule(StageDataVisit stage_data_visit, Player.Data player_data)
		{
			_StageDataVisit = stage_data_visit;
			_PlayerData = player_data;
		}

		public void Run()
		{
			if (_PlayerData.BufferValue < 0)
			{
				_PlayerData.Status = 0;
			}

			if (_PlayerData.Status > 0)
			{
				_PlayerData.Status--;
			}
			else if (AdjustmentPlayerRule.RandNumber >= 200)
			{
				// 20%
				return;
			}

			for (var i = StageDataTable.BufferData.BUFFER_TYPE.NORMAL; i < StageDataTable.BufferData.BUFFER_TYPE.COUNT; ++i)
			{
				var bufferData = _StageDataVisit.FindBufferData(_StageDataVisit.NowUseBlock, i);

				var top = bufferData.Top * bufferData.BufferTempValue.AverageValue;

				if (bufferData.Buffer <= top)
				{
					continue;
				}

				if (AdjustmentPlayerRule.RandNumber < bufferData.Gate)
				{
					bufferData.Buffer -= top;
					_PlayerData.Status = (int)(bufferData.Top * 5);
					_PlayerData.BufferValue = (int)top;
					_PlayerData.Recode.AsnTimes += 1;
				}
				else
				{
					bufferData.Buffer = 0;
				}
			}
		}
	}
}