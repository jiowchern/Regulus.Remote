using System.Collections.Generic;
using System.Linq;


using ProtoBuf;


using Regulus.Utility;

namespace VGame.Project.FishHunter.Common.Data
{
    [ProtoContract]
	public class FishFarmData
	{
		public enum SIZE_TYPE
		{
			SMALL, 

			MEDIUM, 

			LARGE
		}

        [ProtoMember(1)]
		public int FarmId { get; set; }

        [ProtoMember(2)]
        public string Name { get; set; }

        [ProtoMember(3)]
        public SIZE_TYPE SizeType { get; set; }

        [ProtoMember(4)]
        public int BaseOdds { get; set; }

        [ProtoMember(5)]
        public int MaxBet { get; set; }

        [ProtoMember(6)]
        public int GameRate { get; set; }

        [ProtoMember(7)]
        public int NowBaseOdds { get; set; }

        [ProtoMember(8)]
        public int BaseOddsCount { get; set; }

        [ProtoMember(9)]
        public FarmBuffer[] BufferDatas { get; set; }

        [ProtoMember(10)]
        public FarmRecord RecordData { get; set; }

		public FishFarmData()
		{
			var bufferDatas = new List<FarmBuffer>();

			foreach(var i in EnumHelper.GetEnums<FarmBuffer.BUFFER_BLOCK>())
			{
				foreach(var j in EnumHelper.GetEnums<FarmBuffer.BUFFER_TYPE>())
				{
                    bufferDatas.Add(
						new FarmBuffer
						{
							BufferBlock = i, 
							BufferType = j
						});
				}
			}

		    BufferDatas = bufferDatas.ToArray();
		}

		public FarmBuffer FindBuffer(FarmBuffer.BUFFER_BLOCK block, FarmBuffer.BUFFER_TYPE type)
		{
			var data = BufferDatas.First(s => s.BufferBlock == block && s.BufferType == type);
			return data;
		}
	}
}
