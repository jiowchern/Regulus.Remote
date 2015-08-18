using System;

namespace VGame.Project.FishHunter.Common.Data
{
	/// <summary>
	///     The data.
	/// </summary>
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

		public class BufferValue
		{
			public int FireCount { get; set; }

			public int AverageValue { get; set; }

			public int AverageTimes { get; set; }

			public int AverageTotal { get; set; }

			public int HiLoRate { get; set; }

			public int RealTime { get; set; }
		}

		public BUFFER_BLOCK BufferBlock { get; set; }

		public BUFFER_TYPE BufferType { get; set; }

		public int Buffer { get; set; }

		public int Rate { get; set; }

		public int Count { get; set; }

		public int Top { get; set; }

		public int Gate { get; set; }

		public BufferValue BufferTempValue { get; private set; }

		public FarmBuffer()
		{
			BufferTempValue = new BufferValue();
			
		}
	}
}
