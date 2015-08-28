using ProtoBuf;

namespace VGame.Project.FishHunter.Common.Data
{
	[ProtoContract]
	public class RequestWeaponData
	{
		/// <summary>
		///     子彈编号，识别是那颗子弹（网）用的。
		/// </summary>
		[ProtoMember(1)]
		public int BulletId { get; set; }

		/// <summary>
		///     武器形态。一般子弹为“1”。特殊子弹从“2”开始。。。
		/// </summary>
		[ProtoMember(2)]
		public WEAPON_TYPE WeaponType { get; set; }

		/// <summary>
		///     武器分数，就是该子弹的玩家押注分数。从1~1000。(目前無用)
		///		TODO 改用這個值 傳押注比較合理
		/// </summary>
		[ProtoMember(3)]
		public int WeaponBet { get; set; }

		/// <summary>
		///     武器倍数，就是该子弹的玩家押注倍数。从1~1000。
		///		TODO 應該是這個值沒用
		/// </summary>
		[ProtoMember(4)]
		public int WeaponOdds { get; set; }

		/// <summary>
		///     武器同时击中的鱼总数量。应该从1~1000。
		/// </summary>
		[ProtoMember(5)]
		public int TotalHits { get; set; }

		/// <summary>
		/// 目前的押注公式
		/// </summary>
		/// <returns></returns>
		public int GetTotalBet()
		{
			return WeaponOdds * WeaponBet;
		}
	}
}
