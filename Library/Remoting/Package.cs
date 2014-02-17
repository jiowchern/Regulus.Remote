
using System;
namespace Regulus.Remoting
{
    [ProtoBuf.ProtoContract]
    [ProtoBuf.ProtoInclude(1, typeof(Regulus.Utility.Map<byte, byte[]>))]
	public class Package
	{
        [ProtoBuf.ProtoMember(1)]
		public byte Code;
        [ProtoBuf.ProtoMember(2)]
		public Regulus.Utility.Map<byte, byte[]> Args;
	}
}