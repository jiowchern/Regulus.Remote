using ProtoBuf;

namespace VGame.Project.FishHunter.Common.Data
{
    /// <summary>
    ///     擊殺魚的記錄
    /// </summary>
    [ProtoContract]
    public class FishHitRecord
    {
	    [ProtoMember(1)]
        public FISH_TYPE FishType { get; set; }

        [ProtoMember(2)]
        public int KillCount { get; set; }

        [ProtoMember(3)]
        public int WinScore { get; set; }

        [ProtoMember(4)]
        public TreasureRecord[] TreasureRecords { get; set; }

	    public FishHitRecord(FISH_TYPE fish_type)
	    {
		    FishType = fish_type;
		    KillCount = 0;
		    WinScore = 0;
            TreasureRecords = new TreasureRecord[0];
	    }

	    public FishHitRecord()
	    {
			KillCount = 0;
			WinScore = 0;
			TreasureRecords = new TreasureRecord[0];
		}
    }
}
