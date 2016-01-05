using ProtoBuf;

namespace VGame.Project.FishHunter.Common.Data
{
	/// <summary>
	/// ±¼Ä_°O¿ý
	/// </summary>
	[ProtoContract]
	public class TreasureRecord
	{
		[ProtoMember(1)]
		public WEAPON_TYPE WeaponType { get; set; }

		[ProtoMember(2)]
		public int Count { get; set; }

		public TreasureRecord(WEAPON_TYPE type)
		{
			WeaponType = type;
			Count = 0;
		}

		public TreasureRecord()
		{
		}
	}
}
