
using System.Collections.Generic;


using Regulus.Utility;


using VGame.Project.FishHunter.Common.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Data
{
	public class FishFarmBuilder
	{
		public FishFarmData Get(int farm_id)
		{
			// | FarmId | Name | SizeType | BaseOdds | GameRate | MaxBet | NowBaseOdds | BaseChgOddsCnt |
			// | 1       | 魚場1  | SMALL    | 100      | 995      | 1000   | 0           | 0              |
			// 讀檔，取出預設值
			var stageData = new FishFarmData
			{
				FarmId = farm_id, 
				Name = "魚場1", 
				SizeType = FishFarmData.SIZE_TYPE.SMALL, 
				BaseOdds = 100, 
				GameRate = 995, 
				MaxBet = 1000, 
				NowBaseOdds = 0, 
				BaseOddsCount = 0,
				Record = new FarmRecord(farm_id)
			};
			_DefaultBufferData(stageData);

			return stageData;
		}
	
		private void _DefaultBufferData(FishFarmData fish_farm_data)
		{

			foreach(var bufferBlack in EnumHelper.GetEnums<FarmBuffer.BUFFER_BLOCK>())
			{
				var buffer = fish_farm_data.FindBuffer(bufferBlack, FarmBuffer.BUFFER_TYPE.NORMAL);
				_SetDefaultBufferData(buffer, fish_farm_data.GameRate, 0, 0);
					
				buffer = fish_farm_data.FindBuffer(bufferBlack, FarmBuffer.BUFFER_TYPE.SPEC);
				_SetDefaultBufferData(buffer, 20, 3000, 1000);
				
				buffer = fish_farm_data.FindBuffer(bufferBlack, FarmBuffer.BUFFER_TYPE.VIR00);
				_SetDefaultBufferData(buffer, 5, 3000, 1000);
				
				buffer = fish_farm_data.FindBuffer(bufferBlack, FarmBuffer.BUFFER_TYPE.VIR01);
				_SetDefaultBufferData(buffer, 3, 1000, 1000);
				
				buffer = fish_farm_data.FindBuffer(bufferBlack, FarmBuffer.BUFFER_TYPE.VIR02);
				_SetDefaultBufferData(buffer, 1, 1000, 1000);
				
				buffer = fish_farm_data.FindBuffer(bufferBlack, FarmBuffer.BUFFER_TYPE.VIR03);
				_SetDefaultBufferData(buffer, 1, 1000, 1000);
			}
		}

		private void _SetDefaultBufferData(FarmBuffer buffer_data, int rate, int top, int gate)
		{
			buffer_data.Rate = rate;
			buffer_data.Top = top;
			buffer_data.Gate = gate;
		}
	}
}
