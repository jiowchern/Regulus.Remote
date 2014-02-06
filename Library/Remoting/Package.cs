
using System;
namespace Regulus.Remoting
{
	[Serializable]
	public class Package
	{

		public byte Code;


		public Regulus.Utility.Map<byte, byte[]> Args;
	}
}