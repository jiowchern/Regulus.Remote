



namespace VGame.Project.FishHunter.ZsFormula.DataStructs
{
	/// <summary>
	/// The data.
	/// </summary>
	public partial class StageDataTable
	{
		public class BufferData
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

				VIR00, 

				VIR01, 

				VIR02, 

				VIR03, 

				COUNT
			}

			public BUFFER_BLOCK BufferBlock { get; set; }

			public BUFFER_TYPE BufferType { get; set; }

			public long Buffer { get; set; }

			public long Rate { get; set; }

			public long Count { get; set; }

			public long Top { get; set; }

			public long Gate { get; set; }

			public BufferValue BufferTempValue { get; set; }

			public BufferData(
				BUFFER_BLOCK buffer_block, 
				BUFFER_TYPE buffer_type, 
				long buffer, 
				long rate, 
				long count, 
				long top, 
				long gate)
			{
				BufferBlock = buffer_block;
				BufferType = buffer_type;
				Buffer = buffer;
				Rate = rate;
				Count = count;
				Top = top;
				Gate = gate;
				BufferTempValue = new BufferValue();
			}

			public class BufferValue
			{
				public int PlayerTime { get; set; }

				public int AverageValue { get; set; }

				public int AverageTimes { get; set; }

				public int AverageTotal { get; set; }

				public int HiLoRate { get; set; }

				public int RealTime { get; set; }
			}
		}
	}
}