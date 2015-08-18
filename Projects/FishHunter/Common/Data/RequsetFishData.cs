using ProtoBuf;

namespace VGame.Project.FishHunter.Common.Data
{
	[ProtoContract]
	public class RequsetFishData
	{
		/// <summary>
		///     鱼的编号，识别是那只鱼用的。
		/// </summary>
		[ProtoMember(1)]
		public int FishId { get; set; }

		/// <summary>
		///     鱼的倍数。应该从1~1000。目前只预留到1000倍。
		/// </summary>
		[ProtoMember(4)]
		public int FishOdds { get; set; }

		/// <summary>
		///     鱼的状态。
		/// </summary>
		[ProtoMember(3)]
		public FISH_STATUS FishStatus { get; set; }

		/// <summary>
		///     鱼的种类。一般鱼从1~99。特殊鱼从201开始。。。
		/// </summary>
		[ProtoMember(2)]
		public FISH_TYPE FishType { get; set; }
	}
}
