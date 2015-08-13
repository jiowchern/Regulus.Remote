using System;


using ProtoBuf;

namespace VGame.Project.FishHunter.Common.Data
{
	[Flags]
	[ProtoContract]
	public enum FISH_DETERMINATION
	{
		/// <summary>
		///     死亡
		/// </summary>
		DEATH, 

		/// <summary>
		///     存活
		/// </summary>
		SURVIVAL
	}
}
