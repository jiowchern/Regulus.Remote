// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StageDataTable.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the StageDataVisit type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;

using VGame.Project.FishHunter.Common.Datas.FishStage;

namespace VGame.Project.FishHunter.ZsFormula.Data
{
	public class StageDataVisit
	{
		public StageData NowUseData { get; set; }

		public StageBuffer.BUFFER_BLOCK NowUseBlock { get; private set; }

		public StageBuffer FindBuffer(
			StageBuffer.BUFFER_BLOCK block,
			StageBuffer.BUFFER_TYPE type)
		{
			var data = NowUseData.BufferDatas.Select(s => s.BufferBlock == block && s.BufferType == type);
			return data as StageBuffer;
		}

		public StageBuffer.BUFFER_BLOCK GetBufferBlock(int bet)
		{
			var maxBet = NowUseData.MaxBet;

			NowUseBlock = StageBuffer.BUFFER_BLOCK.BLOCK_1;
			if (bet >= ((750 * maxBet) / 1000))
			{
				NowUseBlock = StageBuffer.BUFFER_BLOCK.BLOCK_4;
			}
			else if (bet >= ((500 * maxBet) / 1000))
			{
				NowUseBlock = StageBuffer.BUFFER_BLOCK.BLOCK_3;
			}
			else if (bet >= ((250 * maxBet) / 1000))
			{
				NowUseBlock = StageBuffer.BUFFER_BLOCK.BLOCK_2;
			}
			else if (bet >= ((100 * maxBet) / 1000))
			{
				NowUseBlock = StageBuffer.BUFFER_BLOCK.BLOCK_1;
			}

			return NowUseBlock;
		}
	}
}