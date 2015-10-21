using System;


using ProtoBuf;

namespace VGame.Project.FishHunter.Common.Data
{
	public partial class FarmDataRoot
	{
		[ProtoContract]
		public class BufferNode
		{
			public enum BUFFER_NAME
			{
				NORMAL,

				SPEC,

				VIR00,

				VIR01,

				VIR02,

				VIR03,
			}

			[ProtoMember(1)]
			public BUFFER_NAME BufferName { get; set; }

			/// <summary>
			/// 漁場的輸贏值
			/// </summary>
			[ProtoMember(2)]
			public int WinScore { get; set; } //存錢的地方

			[ProtoMember(3)]
			public int Rate { get; set; }

			[ProtoMember(4)]
			public int Count { get; set; }

			[ProtoMember(5)]
			public int Top { get; set; }

			[ProtoMember(6)]
			public int Gate { get; set; }

			public BufferNode(BUFFER_NAME buffer_name)
			{
				BufferName = buffer_name;
			}

			public BufferNode()
			{
			}
		}
	}
}