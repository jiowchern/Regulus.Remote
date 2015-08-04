// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdjustmentPlayerRule.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   玩家阶段起伏的调整
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System.Linq;


using Regulus.Utility;


using VGame.Project.FishHunter.Common.Datas.FishStage;
using VGame.Project.FishHunter.ZsFormula.Data;

namespace VGame.Project.FishHunter.ZsFormula.Rule
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
			get { return Random.Instance.NextInt(0, 1000); }
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

			//從VIR00 - VIR03
			var enums = EnumHelper.GetEnums<StageBuffer.BUFFER_TYPE>().ToArray();

			for (var i = enums[(int)StageBuffer.BUFFER_TYPE.BUFFER_VIR_BEGIN];
				 i < enums[(int)StageBuffer.BUFFER_TYPE.BUFFER_VIR_END]; ++i)
			{
				var bufferData = _StageDataVisit.FindBuffer(_StageDataVisit.NowUseBlock, i);

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
					_PlayerData.RecodeData.AsnTimes += 1;
				}
				else
				{
					bufferData.Buffer = 0;
				}
			}
		}
	}
}