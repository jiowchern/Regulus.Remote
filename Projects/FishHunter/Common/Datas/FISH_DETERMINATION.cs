using System;

using ProtoBuf;

namespace VGame.Project.FishHunter.Common.Datas
{
	[Flags]
	[ProtoContract]
	public enum FISH_DETERMINATION
	{
		DEATH, 

		SURVIVAL
	}
}