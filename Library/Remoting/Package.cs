using System.Collections.Generic;


using ProtoBuf;

namespace Regulus.Remoting
{
	[ProtoContract]
	[ProtoInclude(1, typeof(Dictionary<byte, byte[]>))]
	public class Package
	{
		[ProtoMember(2)]
		public Dictionary<byte, byte[]> Args;

		[ProtoMember(1)]
		public byte Code;
	}
}
