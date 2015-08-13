using System.Collections.Generic;
using System.Linq;


using Regulus.Utility;

namespace VGame.Project.FishHunter.Common.Data
{
	public class StageData
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

		public List<StageBuffer> BufferDatas { get; set; }

		public StageRecord RecordData { get; set; }

		public StageData()
		{
			BufferDatas = new List<StageBuffer>();
			RecordData = new StageRecord(StageId);

			foreach(var i in EnumHelper.GetEnums<StageBuffer.BUFFER_BLOCK>())
			{
				foreach(var j in EnumHelper.GetEnums<StageBuffer.BUFFER_TYPE>())
				{
					BufferDatas.Add(
						new StageBuffer
						{
							BufferBlock = i, 
							BufferType = j
						});
				}
			}
		}

		public StageBuffer FindBuffer(StageBuffer.BUFFER_BLOCK block, StageBuffer.BUFFER_TYPE type)
		{
			var data = BufferDatas.Select(s => s.BufferBlock == block && s.BufferType == type);
			return data as StageBuffer;
		}
	}
}
