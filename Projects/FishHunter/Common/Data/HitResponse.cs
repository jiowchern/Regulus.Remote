using ProtoBuf;

namespace VGame.Project.FishHunter.Common.Data
{
	/// <summary>
	///     判定结果
	/// </summary>
	[ProtoContract]
	public struct HitResponse
	{
		/// <summary>
		///     死亡结果
		/// </summary>
		[ProtoMember(3)]
		public FISH_DETERMINATION DieResult;

		/// <summary>
		///     取得特殊武器。
		///     0表示没有取得。
		///     有取得特殊武器从“2”开始。对应特武编号。
		/// </summary>
		[ProtoMember(4)]
		public WEAPON_TYPE[] FeedbackWeaponType;

		/// <summary>
		///     鱼的编号
		/// </summary>
		[ProtoMember(2)]
		public int FishID;

		/// <summary>
		///     子弹编号
		/// </summary>
		[ProtoMember(1)]
		public int WepID;

		/// <summary>
		///     1 = 沒有翻倍
		/// </summary>
		[ProtoMember(5)]
		public int WUp;
	}
}
