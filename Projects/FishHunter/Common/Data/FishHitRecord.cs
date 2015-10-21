using ProtoBuf;

namespace VGame.Project.FishHunter.Common.Data
{
    /// <summary>
    ///     魚的口袋，有什麼寶物
    /// </summary>
    [ProtoContract]
    public class FishHitRecord
    {
        [ProtoContract]
        public class TreasureData
        {
			[ProtoMember(1)]
			public WEAPON_TYPE WeaponType { get; set; }

			[ProtoMember(2)]
			public int Count { get; set; }

			public TreasureData(WEAPON_TYPE type)
			{
				WeaponType = type;
				Count = 0;
			}

			public TreasureData()
			{
			}
		}

        [ProtoMember(1)]
        public FISH_TYPE FishType { get; set; }

        [ProtoMember(2)]
        public int KillCount { get; set; }

        [ProtoMember(3)]
        public int WinScore { get; set; }

        [ProtoMember(4)]
        public TreasureData[] TreasureDatas { get; set; }

	    public FishHitRecord(FISH_TYPE fish_type)
	    {
		    FishType = fish_type;
		    KillCount = 0;
		    WinScore = 0;
            TreasureDatas = new TreasureData[0];
	    }

	    public FishHitRecord()
	    {
			KillCount = 0;
			WinScore = 0;
			TreasureDatas = new TreasureData[0];
		}
    }
}
