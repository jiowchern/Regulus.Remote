using System;

using ProtoBuf;

namespace VGame.Project.FishHunter.Common.Data
{
	/// <summary>
	///     The data.
	/// </summary>
	[ProtoContract]
    public partial class FarmDataRoot
	{ 
		[ProtoMember(1)]
		public Guid Id { get; set; }

		[ProtoMember(2)]
		public BlockNode Block { get; set; }

		[ProtoMember(3)]
		public BufferNode Buffer { get; set; }

		[ProtoMember(4)]
		public ValueNode TempValueNode { get; private set; }


		public FarmDataRoot(BlockNode.BLOCK_NAME block_name, BufferNode.BUFFER_NAME buffer_name)
        {
            Id = Guid.NewGuid();

			Block = new BlockNode(block_name);

			Buffer = new BufferNode(buffer_name);

			TempValueNode = new ValueNode();
        }

		public FarmDataRoot()
		{
			Id = Guid.NewGuid();

			Block = new BlockNode();

			Buffer = new BufferNode();

			TempValueNode = new ValueNode();

		}
	}
}
