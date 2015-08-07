using VGame.Project.FishHunter.Common.Data;

namespace VGame.Project.FishHunter.ZsFormula.Data
{
	public class CalculationBufferBlock
	{
		public static StageBuffer.BUFFER_BLOCK? GetBlock(int bet, int max_bet)
		{
			if (bet >= ((750 * max_bet) / 1000))
			{
				return StageBuffer.BUFFER_BLOCK.BLOCK_4;
			}

			if (bet >= ((500 * max_bet) / 1000))
			{
				return StageBuffer.BUFFER_BLOCK.BLOCK_3;
			}

			if (bet >= ((250 * max_bet) / 1000))
			{
				return StageBuffer.BUFFER_BLOCK.BLOCK_2;
			}

			if (bet >= ((100 * max_bet) / 1000))
			{
				return StageBuffer.BUFFER_BLOCK.BLOCK_1;
			}

			return null;
		}
	}
}