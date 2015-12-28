using System;

using Regulus.Utility;

using VGame.Project.FishHunter.Common.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Data
{
	/// <summary>
	///     建立漁場的預設值
	/// </summary>
	public class FishFarmBuilder
	{
		public FishFarmData Get(int farm_id)
		{
			var fishFarmData = _SelectFarm(farm_id);

			fishFarmData.Init();

			_DefaultBufferData(fishFarmData);

			return fishFarmData;
		}

		private FishFarmData _SelectFarm(int farm_id)
		{
			switch(farm_id)
			{
				case 100:
					return new FishFarmData
					{
						FarmId = farm_id, 
						Name = "海洋天堂", 
						MaxBet = 100, 
						GameRate = 992, 
						SpecialRate = 10, 
						BaseOdds = 1000, 
						NowBaseOdds = 300, 
						BaseOddsCount = 0, 
						Record = new FarmRecord(farm_id)
					};
				case 101:
					return new FishFarmData
					{
						FarmId = farm_id, 
						Name = "深海蛇王", 
						MaxBet = 100, 
						GameRate = 994, 
						SpecialRate = 10, 
						BaseOdds = 1000, 
						NowBaseOdds = 300, 
						BaseOddsCount = 0, 
						Record = new FarmRecord(farm_id)
					};
				case 102:
					return new FishFarmData
					{
						FarmId = farm_id, 
						Name = "漁人俱樂部", 
						MaxBet = 1000, 
						GameRate = 996, 
						SpecialRate = 15, 
						BaseOdds = 1000, 
						NowBaseOdds = 300, 
						BaseOddsCount = 0, 
						Record = new FarmRecord(farm_id)
					};
				case 103:
					return new FishFarmData
					{
						FarmId = farm_id, 
						Name = "海洋嘉年華", 
						MaxBet = 1000, 
						GameRate = 997, 
						SpecialRate = 20, 
						BaseOdds = 1000, 
						NowBaseOdds = 300, 
						BaseOddsCount = 0, 
						Record = new FarmRecord(farm_id)
					};
				case 104:
					return new FishFarmData
					{
						FarmId = farm_id, 
						Name = "無敵轟天雷", 
						MaxBet = 9999, 
						GameRate = 998, 
						SpecialRate = 25, 
						BaseOdds = 1000, 
						NowBaseOdds = 300, 
						BaseOddsCount = 0, 
						Record = new FarmRecord(farm_id)
					};

				case 105:
					return new FishFarmData
					{
						FarmId = farm_id, 
						Name = "漁人碼頭", 
						MaxBet = 9999, 
						GameRate = 999, 
						SpecialRate = 30, 
						BaseOdds = 1000, 
						NowBaseOdds = 300, 
						BaseOddsCount = 0, 
						Record = new FarmRecord(farm_id)
					};

				// 以下昌盛版本
				case 106:
					return new FishFarmData
					{
						FarmId = farm_id, 
						Name = "鱼乐无穷", 
						MaxBet = 100, 
						GameRate = 992, 
						SpecialRate = 10, 
						BaseOdds = 1000, 
						NowBaseOdds = 300, 
						BaseOddsCount = 0, 
						Record = new FarmRecord(farm_id)
					};
				case 107:
					return new FishFarmData
					{
						FarmId = farm_id, 
						Name = "火山巨蛇", 
						MaxBet = 100, 
						GameRate = 994, 
						SpecialRate = 10, 
						BaseOdds = 1000, 
						NowBaseOdds = 300, 
						BaseOddsCount = 0, 
						Record = new FarmRecord(farm_id)
					};
				case 108:
					return new FishFarmData
					{
						FarmId = farm_id, 
						Name = "大鱼吃小鱼", 
						MaxBet = 1000, 
						GameRate = 996, 
						SpecialRate = 15, 
						BaseOdds = 1000, 
						NowBaseOdds = 300, 
						BaseOddsCount = 0, 
						Record = new FarmRecord(farm_id)
					};
				case 109:
					return new FishFarmData
					{
						FarmId = farm_id, 
						Name = "破坏铁球", 
						MaxBet = 1000, 
						GameRate = 997, 
						SpecialRate = 20, 
						BaseOdds = 1000, 
						NowBaseOdds = 300, 
						BaseOddsCount = 0, 
						Record = new FarmRecord(farm_id)
					};
				case 110:
					return new FishFarmData
					{
						FarmId = farm_id, 
						Name = "群鲸来袭", 
						MaxBet = 9999, 
						GameRate = 998, 
						SpecialRate = 25, 
						BaseOdds = 1000, 
						NowBaseOdds = 300, 
						BaseOddsCount = 0, 
						Record = new FarmRecord(farm_id)
					};

				case 111:
					return new FishFarmData
					{
						FarmId = farm_id, 
						Name = "七彩霸王鲸", 
						MaxBet = 9999, 
						GameRate = 999, 
						SpecialRate = 30, 
						BaseOdds = 1000, 
						NowBaseOdds = 300, 
						BaseOddsCount = 0, 
						Record = new FarmRecord(farm_id)
					};

				default:
					throw new Exception("无此编号的渔场");
			}
		}

		private void _DefaultBufferData(FishFarmData fish_farm_data)
		{
			foreach(var bufferBlock in EnumHelper.GetEnums<FarmDataRoot.BlockNode.BLOCK_NAME>())
			{
				var dataRoot = fish_farm_data.FindDataRoot(bufferBlock, FarmDataRoot.BufferNode.BUFFER_NAME.NORMAL);
				_SetDefaultBufferData(dataRoot, fish_farm_data.GameRate - fish_farm_data.SpecialRate, 0, 0);

				dataRoot = fish_farm_data.FindDataRoot(bufferBlock, FarmDataRoot.BufferNode.BUFFER_NAME.SPEC);
				_SetDefaultBufferData(dataRoot, fish_farm_data.SpecialRate, 3000, 1000);

				dataRoot = fish_farm_data.FindDataRoot(bufferBlock, FarmDataRoot.BufferNode.BUFFER_NAME.VIR00);
				_SetDefaultBufferData(dataRoot, 5, 3000, 1000);

				dataRoot = fish_farm_data.FindDataRoot(bufferBlock, FarmDataRoot.BufferNode.BUFFER_NAME.VIR01);
				_SetDefaultBufferData(dataRoot, 3, 1000, 1000);

				dataRoot = fish_farm_data.FindDataRoot(bufferBlock, FarmDataRoot.BufferNode.BUFFER_NAME.VIR02);
				_SetDefaultBufferData(dataRoot, 1, 1000, 1000);

				dataRoot = fish_farm_data.FindDataRoot(bufferBlock, FarmDataRoot.BufferNode.BUFFER_NAME.VIR03);
				_SetDefaultBufferData(dataRoot, 1, 1000, 1000);
			}
		}

		private void _SetDefaultBufferData(FarmDataRoot farm_data_root, int rate, int top, int gate)
		{
			farm_data_root.Buffer.Rate = rate;
			farm_data_root.Buffer.Top = top;
			farm_data_root.Buffer.Gate = gate;
		}
	}
}
