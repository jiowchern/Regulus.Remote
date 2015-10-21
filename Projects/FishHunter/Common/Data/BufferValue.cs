using System;

using ProtoBuf;

namespace VGame.Project.FishHunter.Common.Data
{
	public partial class FarmDataRoot
	{
		[ProtoContract]
		public class ValueNode
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

			public ValueNode()
			{
			}
		}
	}
}