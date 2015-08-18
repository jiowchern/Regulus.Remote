using ProtoBuf;


using Regulus.Utility;

namespace VGame.Project.FishHunter.Common.Data
{
    [ProtoContract]
    public class FarmRecord
	{
        [ProtoMember(1)]
        public int FarmId { get; set; }

        [ProtoMember(2)]
        public int PlayTotal { get; set; } // 0

        [ProtoMember(3)]
        public int WinScore { get; set; }

        [ProtoMember(4)]
        public int PlayTimes { get; set; }

        [ProtoMember(5)]
        public int WinFrequency { get; set; }

        [ProtoMember(6)]
        public int AsnTimes { get; set; }

        [ProtoMember(7)]
        public int AsnWin { get; set; }

        [ProtoContract]
        public class HitHistory
		{
			public HitHistory(FISH_TYPE fish, int kill_conunt)
			{
				FishType = fish;
				KillCount = kill_conunt;
			}

            [ProtoMember(1)]
            public FISH_TYPE FishType { get; private set; }

            [ProtoMember(2)]
            public int KillCount { get; set; }
		}

        [ProtoMember(8)]
        public FishPocket FishHitReuslt { get; set; }

		public FarmRecord(int farm_id)
		{
			FarmId = farm_id;
			FishHitReuslt = new FishPocket();
		}

	}
}
