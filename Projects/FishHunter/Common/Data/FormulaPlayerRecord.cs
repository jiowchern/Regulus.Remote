using System;
using System.Collections.Generic;
using System.Linq;


using ProtoBuf;

namespace VGame.Project.FishHunter.Common.Data
{
    /// <summary>
    /// 算法 player 記錄
    /// </summary>
    [ProtoContract]
    public class FormulaPlayerRecord
	{
        [ProtoMember(1)]
		public Guid Id { get; set; }

        [ProtoMember(2)]
        public Guid Owner { get; set; }

        [ProtoMember(3)]
        public int Status { get; set; }

        [ProtoMember(4)]
        public int BufferValue { get; set; }

        [ProtoMember(5)]
        public FarmRecord[] StageRecords { get; set; }

        public FormulaPlayerRecord()
		{
			StageRecords = new List<FarmRecord>().ToArray();
		}

        public FarmRecord FindFarmRecord(int farm_id)
		{			
			return StageRecords.First(x => x.FarmId == farm_id);
		}
	}
}