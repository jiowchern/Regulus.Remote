using System;


using ProtoBuf;

namespace VGame.Project.FishHunter.Common.Data
{
	/// <summary>
	///     鱼的状态。
	/// </summary>
	[Flags]
	[ProtoContract]
	public enum FISH_STATUS
	{
		/// <summary>
		///     正常状态
		/// </summary>
		NORMAL, 

		/// <summary>
		///     鱼王状态
		/// </summary>
		KING, 

		/// <summary>
		///     冰冻状态
		/// </summary>
		FREEZE
	}
}