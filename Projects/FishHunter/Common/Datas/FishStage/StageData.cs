using System.Collections.Generic;

namespace VGame.Project.FishHunter.Common.Datas.FishStage
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

		public StageRecode RecodeData { get; set; }

		public StageData()
		{
			BufferDatas = new List<StageBuffer>();
		}
	}
}