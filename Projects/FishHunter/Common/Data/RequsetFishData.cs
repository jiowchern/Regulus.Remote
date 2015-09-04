// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequsetFishData.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the RequsetFishData type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using ProtoBuf;

namespace VGame.Project.FishHunter.Common.Data
{
	/// <summary>
	/// 魚的封包定義
	/// </summary>
	[ProtoContract]
	public class RequsetFishData
	{
		/// <summary>
		///     鱼的编号，识别是那只鱼用的。
		/// </summary>
		[ProtoMember(1)]
		public int FishId { get; set; }

		/// <summary>
		/// 鱼的种类。一般鱼从1~99。特殊鱼从201开始。。。
		/// </summary>
		[ProtoMember(2)]
		public FISH_TYPE FishType { get; set; }

		/// <summary>
		///     鱼的状态。
		/// </summary>
		[ProtoMember(3)]
		public FISH_STATUS FishStatus { get; set; }

		/// <summary>
		///     鱼的倍数。应该从1~1000。目前只预留到1000倍。
		/// </summary>
		[ProtoMember(4)]
		public int FishOdds { get; set; }

		/// <summary>
		/// 陪葬品
		/// 條件觸發後，跟發動者一起死
		/// 例如 炸彈炸到的魚、魚王的小魚
		/// </summary>
		[ProtoMember(5)]
		public RequsetFishData[] GraveGoods { get; set; }
	}
}
