
using System;
namespace Regulus.Remoting
{
    [ProtoBuf.ProtoContract]
    
    [ProtoBuf.ProtoInclude(1, typeof(System.Collections.Generic.Dictionary<byte, byte[]>))]
	public class Package
	{
        [ProtoBuf.ProtoMember(1)]
		public byte Code;
        [ProtoBuf.ProtoMember(2)]
        public System.Collections.Generic.Dictionary<byte, byte[]> Args;
	}
}