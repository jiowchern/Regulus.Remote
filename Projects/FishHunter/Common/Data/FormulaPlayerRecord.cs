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
        public int Status { get; set; } //開啟好贏計數器、flag，(倒數)

        [ProtoMember(4)]
        public int BufferValue { get; set; }

        [ProtoMember(5)]
        public FarmRecord[] FarmRecords { get; set; }

        public FormulaPlayerRecord()
        {
	        FarmRecords = new FarmRecord[0];
        }

        public FarmRecord FindFarmRecord(int farm_id)
		{			
			return FarmRecords.FirstOrDefault(x => x.FarmId == farm_id);
		}
	}
}