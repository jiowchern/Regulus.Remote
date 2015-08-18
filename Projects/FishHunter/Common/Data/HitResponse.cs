using ProtoBuf;

namespace VGame.Project.FishHunter.Common.Data
{
	/// <summary>
	///     判定结果
	/// </summary>
	[ProtoContract]
	public class HitResponse
	{
		/// <summary>
		///     子弹编号
		/// </summary>
		[ProtoMember(1)]
		public int WepId { get; set; }

		/// <summary>
		///     鱼的编号
		/// </summary>
		[ProtoMember(2)]
		public int FishId { get; set; }

		/// <summary>
		///     死亡结果
		/// </summary>
		[ProtoMember(3)]
		public FISH_DETERMINATION DieResult { get; set; }

		/// <summary>
		///     取得特殊武器。
		///     0表示没有取得。
		///     有取得特殊武器从“2”开始。对应特武编号。
		/// </summary>
		[ProtoMember(4)]
		public WEAPON_TYPE[] FeedbackWeaponType { get; set; }

		/// <summary>
		///     1 = 沒有翻倍
		/// </summary>
		[ProtoMember(5)]
		public int WUp { get; set; }
	}
}
