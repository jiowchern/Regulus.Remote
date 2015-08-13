using ProtoBuf;

namespace VGame.Project.FishHunter.Common.Data
{
	public struct RequestWeaponData
	{
		/// <summary>
		///     武器同时击中的鱼倍数总和。
		/// </summary>
		[ProtoMember(6)]
		public int TotalHitOdds;

		/// <summary>
		///     武器同时击中的鱼总数量。应该从1~1000。
		/// </summary>
		[ProtoMember(5)]
		public int TotalHits;

		/// <summary>
		///     武器形态。一般子弹为“1”。特殊子弹从“2”开始。。。
		/// </summary>
		[ProtoMember(2)]
		public WEAPON_TYPE WeaponType;

		/// <summary>
		///     武器分数，就是该子弹的玩家押注分数。从1~10000。
		/// </summary>
		[ProtoMember(3)]
		public int WepBet;

		/// <summary>
		///     子彈编号，识别是那颗子弹（网）用的。
		/// </summary>
		[ProtoMember(1)]
		public int WepID;

		/// <summary>
		///     武器倍数，就是该子弹的玩家押注倍数。从1~10000。
		/// </summary>
		[ProtoMember(4)]
		public int WepOdds;
	}
}
