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
        }

        [ProtoMember(1)]
        public FISH_TYPE FishType { get; set; }

        [ProtoMember(2)]
        public int KillCount { get; set; }

        [ProtoMember(3)]
        public int WinScore { get; set; }

        [ProtoMember(4)]
        public TreasureData[] Datas { get; set; }
    }
}
