using System;


using ProtoBuf;

namespace VGame.Project.FishHunter.Common.Data
{
	/// <summary>
	///     game player記錄資料
	/// </summary>
	[ProtoContract]
	public class GamePlayerRecord
	{
		[ProtoMember(1)]
		public Guid Id { get; set; }

		[ProtoMember(2)]
		public int Money { get; set; }

		[ProtoMember(3)]
		public Guid Owner { get; set; }
	}

}

