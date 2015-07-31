// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StageDataTable.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the StageDataTable type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System.Collections.Generic;
using System.Linq;

namespace VGame.Project.FishHunter.ZsFormula.DataStructs
{
	public class StageDataVisit
	{
		public StageDataTable.Data NowUseData { get; set; }

		public StageDataTable.BufferData.BUFFER_BLOCK NowUseBlock { get; private set; }

		public StageDataTable.BufferData FindBufferData(
			StageDataTable.BufferData.BUFFER_BLOCK block, 
			StageDataTable.BufferData.BUFFER_TYPE type)
		{
			var data = NowUseData.BufferDatas.Select(s => s.BufferBlock == block && s.BufferType == type);
			return data as StageDataTable.BufferData;
		}

		public StageDataTable.BufferData.BUFFER_BLOCK GetBufferBlock(int bet)
		{
			var maxBet = NowUseData.MaxBet;

			NowUseBlock = StageDataTable.BufferData.BUFFER_BLOCK.BLOCK_1;
			if (bet >= ((750 * maxBet) / 1000))
			{
				NowUseBlock = StageDataTable.BufferData.BUFFER_BLOCK.BLOCK_4;
			}
			else if (bet >= ((500 * maxBet) / 1000))
			{
				NowUseBlock = StageDataTable.BufferData.BUFFER_BLOCK.BLOCK_3;
			}
			else if (bet >= ((250 * maxBet) / 1000))
			{
				NowUseBlock = StageDataTable.BufferData.BUFFER_BLOCK.BLOCK_2;
			}
			else if (bet >= ((100 * maxBet) / 1000))
			{
				NowUseBlock = StageDataTable.BufferData.BUFFER_BLOCK.BLOCK_1;
			}

			return NowUseBlock;
		}
	}

	public class StageDataBuilder
	{
		private readonly StageDataTable _StageDataTable;

		private readonly StageDataVisit _StageDataVisit;

		public StageDataBuilder()
		{
			_StageDataTable = new StageDataTable();
			_StageDataVisit = new StageDataVisit();

			_InitBufferData();
		}

		private void _InitBufferData()
		{
			foreach (var stageData in _StageDataTable.TableDatas.Values)
			{
				_StageDataVisit.NowUseData = stageData;

				for (var i = StageDataTable.BufferData.BUFFER_BLOCK.BLOCK_1; i < StageDataTable.BufferData.BUFFER_BLOCK.COUNT; ++i)
				{
					for (var j = StageDataTable.BufferData.BUFFER_TYPE.NORMAL; j < StageDataTable.BufferData.BUFFER_TYPE.COUNT; ++j)
					{
						var data = _StageDataVisit.FindBufferData(i, j);

						_SetDefaultBufferData(data, _StageDataVisit.NowUseData.GameRate, 0, 0);

						_SetDefaultBufferData(data, 20, 3000, 1000);

						_SetDefaultBufferData(data, 5, 3000, 1000);

						_SetDefaultBufferData(data, 3, 1000, 1000);

						_SetDefaultBufferData(data, 1, 1000, 1000);

						_SetDefaultBufferData(data, 1, 1000, 1000);
					}
				}
			}
		}

		private void _SetDefaultBufferData(StageDataTable.BufferData buffer_data, int rate, int top, int gate)
		{
			buffer_data.Rate = rate;
			buffer_data.Top = top;
			buffer_data.Gate = gate;
		}
	}

	/// <summary>
	/// 一個stage裡面有n個buffer
	/// </summary>
	public partial class StageDataTable
	{
		public Dictionary<int, Data> TableDatas { get; private set; }

		public StageDataTable(IEnumerable<Data> table_datas)
		{
			TableDatas = table_datas.ToDictionary(x => x.StageId);
		}

		public StageDataTable()
		{
		}

		public class Data
		{
			public enum SIZE_TYPE
			{
				SMALL, 

				MEDIUM, 

				LARGE
			}

			public int StageId { get; set; }

			public string Name { get; set; }

			public SIZE_TYPE SizeType { get; set; }

			public int BaseOdds { get; set; }

			public int MaxBet { get; set; }

			public int GameRate { get; set; }

			public int NowBaseOdds { get; set; }

			public int BaseOddsCount { get; set; }

			public List<BufferData> BufferDatas { get; set; }

			public Recode Recode { get; set; }

			public Data()
			{
				BufferDatas = new List<BufferData>();
			}
		}
	}
}