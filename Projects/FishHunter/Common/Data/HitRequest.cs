using ProtoBuf;

namespace VGame.Project.FishHunter.Common.Data
{
	/// <summary>
	///     新版的封包定義
	/// </summary>
	[ProtoContract]
	public class HitRequest
	{
		/// <summary>
		///     魚的資料
		/// </summary>
		[ProtoMember(1)]
		public RequsetFishData[] FishDatas { get; set; }

		/// <summary>
		///     武器資料
		/// </summary>
		[ProtoMember(2)]
		public RequestWeaponData WeaponData { get; set; }

		/// <summary>
		///     判斷武器是否第一次觸發
		/// </summary>
		[ProtoMember(3)]
		public bool IsFirstTrigger { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="HitRequest"/> class. 
		/// </summary>
		public HitRequest()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HitRequest"/> class. 
		/// </summary>
		public HitRequest(RequsetFishData[] fish_datas, RequestWeaponData weapon_data, bool is_first_trigger)
		{
			IsFirstTrigger = is_first_trigger;
			FishDatas = fish_datas;
			WeaponData = weapon_data;
		}
	}
}
