using ProtoBuf;

namespace VGame.Project.FishHunter.Common.Data
{
	/// <summary>
	///     新版的封包定義
	/// </summary>
	[ProtoContract]
	public class HitRequest
	{
		[ProtoMember(1)]
		public RequsetFishData[] FishDatas { get; set; }

		[ProtoMember(2)]
		public RequestWeaponData WeaponData { get; set; }

		public HitRequest()
		{
		}

		public HitRequest(RequsetFishData[] fish_datas, RequestWeaponData weapon_data)
		{
			FishDatas = fish_datas;
			WeaponData = weapon_data;
		}
	}
}
