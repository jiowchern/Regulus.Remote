using System;


using ProtoBuf;

namespace VGame.Project.FishHunter.Common.Data
{
	[Flags]
	[ProtoContract]
	public enum FISH_DETERMINATION
	{
		DEATH, 

		SURVIVAL
	}
}