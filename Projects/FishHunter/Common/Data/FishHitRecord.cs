using ProtoBuf;

namespace VGame.Project.FishHunter.Common.Data
{
	/// <summary>
	///     擊殺魚的記錄
	/// </summary>
	[ProtoContract]
	public class WeaponHitRecord
	{
		/// <summary>
		///     武器
		/// </summary>
		[ProtoMember(1)]
		public WEAPON_TYPE WeaponType { get; set; }

		/// <summary>
		///     魚種
		/// </summary>
		[ProtoMember(2)]
		public FishKillRecord[] FishKills { get; set; }

		/// <summary>
		///     倍數總合
		/// </summary>
		[ProtoMember(3)]
		public int TotalOdds { get; set; }

		/// <summary>
		///     贏分
		/// </summary>
		[ProtoMember(4)]
		public int WinScore { get; set; }

		/// <summary>
		///     Initializes a new instance of the <see cref="WeaponHitRecord" /> class.
		/// </summary>
		public WeaponHitRecord(WEAPON_TYPE weapon_type)
		{
			WeaponType = weapon_type;
			FishKills = new FishKillRecord[0];
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="WeaponHitRecord" /> class.
		/// </summary>
		public WeaponHitRecord()
		{
			FishKills = new FishKillRecord[0];
		}
	}
}
