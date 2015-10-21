using ProtoBuf;

namespace VGame.Project.FishHunter.Common.Data
{
	public partial class FarmDataRoot
	{
		[ProtoContract]
		public class BlockNode
		{
			public enum BLOCK_NAME
			{
				BLOCK_1,

				BLOCK_2,

				BLOCK_3,

				BLOCK_4,

				BLOCK_5,
			}

			[ProtoMember(1)]
			public BLOCK_NAME BlockName { get; set; }

			[ProtoMember(2)]
			public int TotalSpending { get; set; }

			[ProtoMember(3)]
			public int FireCount { get; set; }

			[ProtoMember(4)]
			public int WinScore { get; set; }

			public BlockNode(BLOCK_NAME block_name)
			{
				BlockName = block_name;
			}

			public BlockNode()
			{
			}
		}
	}
}