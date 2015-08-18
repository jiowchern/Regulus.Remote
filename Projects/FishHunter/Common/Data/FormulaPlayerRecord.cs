using System;
using System.Collections.Generic;

namespace VGame.Project.FishHunter.Common.Data
{
	/// <summary>
	/// 算法 player 記錄
	/// </summary>
	public class FormulaPlayerRecord
	{
		public Guid Id { get; set; }

		public Guid Owner { get; set; }

		public int Status { get; set; }

		public int BufferValue { get; set; }

		public List<FarmRecord> StageRecords { get; set; }

		public FormulaPlayerRecord()
		{
			StageRecords = new List<FarmRecord>();
		}

		public FarmRecord FindStageRecord(int stage_id)
		{			
			return StageRecords.Find(x => x.FarmId == stage_id);
		}
	}
}