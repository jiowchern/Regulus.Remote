using ProtoBuf;

namespace VGame.Project.FishHunter.Common.Data
{
	/// <summary>
	///     À»±þ°O¿ý
	/// </summary>
	[ProtoContract]
	public class FishKillRecord
	{
		[ProtoMember(1)]
		public FISH_TYPE FishType { get; set; }

		[ProtoMember(2)]
		public int KillCount { get; set; }

		public FishKillRecord()
		{
			KillCount = 0;
		}

		public FishKillRecord(FISH_TYPE type)
		{
			FishType = type;
			KillCount = 0;
		}
	}
}
