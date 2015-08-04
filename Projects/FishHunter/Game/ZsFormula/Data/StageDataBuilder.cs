using System.Collections.Generic;


using VGame.Project.FishHunter.Common.Datas.FishStage;

namespace VGame.Project.FishHunter.ZsFormula.Data
{
	public class StageDataBuilder
	{
		private readonly StageDataVisit _StageDataVisit;

		public StageDataBuilder(IEnumerable<StageData> datas)
		{
			_StageDataVisit = new StageDataVisit();

			_InitBufferData(datas);
		}

		private void _InitBufferData(IEnumerable<StageData> datas)
		{
			foreach (var d in datas)
			{
				_StageDataVisit.NowUseData = d;

				for (var i = StageBuffer.BUFFER_BLOCK.BLOCK_1; i < StageBuffer.BUFFER_BLOCK.COUNT; ++i)
				{
					for (var j = StageBuffer.BUFFER_TYPE.NORMAL; j < StageBuffer.BUFFER_TYPE.COUNT; ++j)
					{
						var buffer = _StageDataVisit.FindBuffer(i, j);

						_SetDefaultBufferData(buffer, _StageDataVisit.NowUseData.GameRate, 0, 0);

						_SetDefaultBufferData(buffer, 20, 3000, 1000);

						_SetDefaultBufferData(buffer, 5, 3000, 1000);

						_SetDefaultBufferData(buffer, 3, 1000, 1000);

						_SetDefaultBufferData(buffer, 1, 1000, 1000);

						_SetDefaultBufferData(buffer, 1, 1000, 1000);
					}
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