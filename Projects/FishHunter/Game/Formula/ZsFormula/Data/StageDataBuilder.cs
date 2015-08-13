
using VGame.Project.FishHunter.Common.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Data
{
	public class StageDataBuilder
	{
		public StageData Get(int stage_id)
		{
			// | StageId | Name | SizeType | BaseOdds | GameRate | MaxBet | NowBaseOdds | BaseChgOddsCnt |
			// | 1       | 魚場1  | SMALL    | 100      | 995      | 1000   | 0           | 0              |
			// 讀檔，取出預設值
			var stageData = new StageData
			{
				StageId = stage_id, 
				Name = "魚場1", 
				SizeType = StageData.SIZE_TYPE.SMALL, 
				BaseOdds = 100, 
				GameRate = 995, 
				MaxBet = 1000, 
				NowBaseOdds = 0, 
				BaseOddsCount = 0
			};
			_DefaultBufferData(stageData);

			return stageData;
		}

		private void _DefaultBufferData(StageData stage_data)
		{
			for(var i = StageBuffer.BUFFER_BLOCK.BLOCK_1; i < StageBuffer.BUFFER_BLOCK.COUNT; ++i)
			{
				for(var j = StageBuffer.BUFFER_TYPE.NORMAL; j < StageBuffer.BUFFER_TYPE.COUNT; ++j)
				{
					var buffer = stage_data.FindBuffer(i, j);

					_SetDefaultBufferData(buffer, stage_data.GameRate, 0, 0);

					_SetDefaultBufferData(buffer, 20, 3000, 1000);

					_SetDefaultBufferData(buffer, 5, 3000, 1000);

					_SetDefaultBufferData(buffer, 3, 1000, 1000);

					_SetDefaultBufferData(buffer, 1, 1000, 1000);

					_SetDefaultBufferData(buffer, 1, 1000, 1000);
				}
			}
		}

		private void _SetDefaultBufferData(StageBuffer buffer_data, int rate, int top, int gate)
		{
			buffer_data.Rate = rate;
			buffer_data.Top = top;
			buffer_data.Gate = gate;
		}
	}
}
