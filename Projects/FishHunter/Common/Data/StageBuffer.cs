using System;

namespace VGame.Project.FishHunter.Common.Data
{
	/// <summary>
	///     The data.
	/// </summary>
	public class StageBuffer
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

		public BufferValue BufferTempValue { get; set; }

		public StageBuffer(BUFFER_BLOCK buffer_block, BUFFER_TYPE buffer_type)
		{
			BufferBlock = buffer_block;
			BufferType = buffer_type;
			Buffer = 0;
			Rate = 0;
			Count = 0;
			Top = 0;
			Gate = 0;

			BufferTempValue = new BufferValue();
		}

		public StageBuffer()
		{
			throw new NotImplementedException();
		}
	}
}
