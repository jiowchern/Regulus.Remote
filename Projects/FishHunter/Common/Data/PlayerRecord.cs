using System;
using System.Collections.Generic;


using ProtoBuf;

namespace VGame.Project.FishHunter.Common.Data
{
	/// <summary>
	///     玩家記錄資料
	/// </summary>
	[ProtoContract]
	public class PlayerRecord
	{
		[ProtoMember(1)]
		public Guid Id { get; set; }

		[ProtoMember(2)]
		public int Money { get; set; }

		[ProtoMember(3)]
		public Guid Owner { get; set; }

		public int Status { get; set; }

		public int BufferValue { get; set; }

		public SpecialWeaponData NowSpecialWeaponData { get; set; }

		public List<StageRecord> StageRecord { get; set; }

		public PlayerRecord()
		{
			Id = Guid.NewGuid();
			Owner = Guid.Empty;
			StageRecord = new List<StageRecord>();
		}

		public PlayerRecord(Guid id)
		{
			Id = id;
			Owner = Guid.Empty;
			StageRecord = new List<StageRecord>();
		}

		public StageRecord FindStageRecord(int stage_id)
		{
			return StageRecord.Find(x => x.StageId == stage_id);
		}
	}
}
