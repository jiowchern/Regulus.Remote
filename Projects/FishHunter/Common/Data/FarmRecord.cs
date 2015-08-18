using Regulus.Utility;

namespace VGame.Project.FishHunter.Common.Data
{
	public class FarmRecord
	{
		public int FarmId { get; set; }

		public int PlayTotal { get; set; } // 0

		public int WinScore { get; set; }

		public int PlayTimes { get; set; }

		public int WinFrequency { get; set; }

		public int AsnTimes { get; set; }

		public int AsnWin { get; set; }

		public class HitHistory
		{
			public HitHistory(FISH_TYPE fish, int kill_conunt)
			{
				FishType = fish;
				KillCount = kill_conunt;
			}

			public FISH_TYPE FishType { get; private set; }

			public int KillCount { get; set; }
		}

		public FishPocket FishHitReuslt { get; set; }

		public FarmRecord(int farm_id)
		{
			FarmId = farm_id;
			FishHitReuslt = new FishPocket();
		}

	}
}
