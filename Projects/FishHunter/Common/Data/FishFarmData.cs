using System;
using System.Collections.Generic;
using System.Linq;


using ProtoBuf;


using Regulus.Utility;

namespace VGame.Project.FishHunter.Common.Data
{
	[ProtoContract]
	public class FishFarmData
	{
		public enum SIZE_TYPE
		{
			SMALL, 

			MEDIUM, 

			LARGE
		}

		[ProtoMember(1)]
		public Guid Id { get; set; }

		[ProtoMember(2)]
		public int FarmId { get; set; }

		[ProtoMember(3)]
		public string Name { get; set; }

		[ProtoMember(4)]
		public int BaseOdds { get; set; }

		[ProtoMember(5)]
		public int MaxBet { get; set; }

		[ProtoMember(6)]
		public int GameRate { get; set; }

		[ProtoMember(7)]
		public int SpecialRate { get; set; }
		

		[ProtoMember(8)]
		public int NowBaseOdds { get; set; }

		[ProtoMember(9)]
		public int BaseOddsCount { get; set; }

		[ProtoMember(10)]
		public FarmDataRoot[] DataRootRoots { get; set; }

		[ProtoMember(11)]
		public FarmRecord Record { get; set; }

		public void Init()
		{
			Id = Guid.NewGuid();

			DataRootRoots = (from i in EnumHelper.GetEnums<FarmDataRoot.BlockNode.BLOCK_NAME>()
								from j in EnumHelper.GetEnums<FarmDataRoot.BufferNode.BUFFER_NAME>()
								select new FarmDataRoot(i, j)).ToArray();
		}

		public FarmDataRoot FindDataRoot(FarmDataRoot.BlockNode.BLOCK_NAME block_name, FarmDataRoot.BufferNode.BUFFER_NAME buffer_name)
		{
			var data = DataRootRoots.First(s => s.Block.BlockName == block_name && s.Buffer.BufferName == buffer_name);
			return data;
		}
	}
}
