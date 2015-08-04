// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Package.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Package type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.Collections.Generic;

using ProtoBuf;

#endregion

namespace Regulus.Remoting
{
	[ProtoContract]
	[ProtoInclude(1, typeof (Dictionary<byte, byte[]>))]
	public class Package
	{
		[ProtoMember(2)]
		public Dictionary<byte, byte[]> Args;

		[ProtoMember(1)]
		public byte Code;
	}
}