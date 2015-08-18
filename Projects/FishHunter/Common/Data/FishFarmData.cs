using System.Collections.Generic;
using System.Linq;


using Regulus.Utility;

namespace VGame.Project.FishHunter.Common.Data
{
	public class FishFarmData
	{
		public enum SIZE_TYPE
		{
			SMALL, 

			MEDIUM, 

			LARGE
		}

		public int FarmId { get; set; }

		public string Name { get; set; }

		public SIZE_TYPE SizeType { get; set; }

		public int BaseOdds { get; set; }

		public int MaxBet { get; set; }

		public int GameRate { get; set; }

		public int NowBaseOdds { get; set; }

		public int BaseOddsCount { get; set; }

		public List<FarmBuffer> BufferDatas { get; set; }

		public FarmRecord RecordData { get; set; }

		public FishFarmData()
		{
			BufferDatas = new List<FarmBuffer>();

			foreach(var i in EnumHelper.GetEnums<FarmBuffer.BUFFER_BLOCK>())
			{
				foreach(var j in EnumHelper.GetEnums<FarmBuffer.BUFFER_TYPE>())
				{
					BufferDatas.Add(
						new FarmBuffer
						{
							BufferBlock = i, 
							BufferType = j
						});
				}
			}
		}

		public FarmBuffer FindBuffer(FarmBuffer.BUFFER_BLOCK block, FarmBuffer.BUFFER_TYPE type)
		{
			var data = BufferDatas.First(s => s.BufferBlock == block && s.BufferType == type);
			return data;
		}
	}
}
