using System;


using ProtoBuf;

namespace VGame.Project.FishHunter.Common.Data
{
    /// <summary>
    ///     The data.
    /// </summary>
    /// [ProtoContract]
    [ProtoContract]
    public class FarmBuffer
	{
		public enum BUFFER_BLOCK
		{
			BLOCK_1, 

			BLOCK_2, 

			BLOCK_3, 

			BLOCK_4, 

			BLOCK_5, 

			COUNT
		}

		public enum BUFFER_TYPE
		{
			NORMAL, 

			SPEC, 

			BUFFER_VIR_BEGIN, 

			VIR00 = BUFFER_TYPE.BUFFER_VIR_BEGIN, 

			VIR01, 

			VIR02, 

			VIR03, 

			BUFFER_VIR_END, 

			COUNT = BUFFER_TYPE.BUFFER_VIR_END
		}

        [ProtoContract]
        public class BufferValue
		{
            [ProtoMember(1)]
            public int FireCount { get; set; }

            [ProtoMember(2)]
            public int AverageValue { get; set; }

            [ProtoMember(3)]
            public int AverageTimes { get; set; }

            [ProtoMember(4)]
            public int AverageTotal { get; set; }

            [ProtoMember(5)]
            public int HiLoRate { get; set; }

            [ProtoMember(6)]
            public int RealTime { get; set; }
		}

        [ProtoMember(1)]
		public BUFFER_BLOCK BufferBlock { get; set; }

        [ProtoMember(2)]
        public BUFFER_TYPE BufferType { get; set; }

        [ProtoMember(3)]
        public int Buffer { get; set; }

        [ProtoMember(4)]
        public int Rate { get; set; }

        [ProtoMember(5)]
        public int Count { get; set; }

        [ProtoMember(6)]
        public int Top { get; set; }

        [ProtoMember(7)]
        public int Gate { get; set; }

        [ProtoMember(8)]
        public BufferValue BufferTempValue { get; private set; }

		public FarmBuffer()
		{
			BufferTempValue = new BufferValue();
			
		}
	}
}
