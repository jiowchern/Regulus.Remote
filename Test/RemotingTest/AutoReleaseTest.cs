using System;
using System.Collections.Generic;





using NSubstitute;
using Regulus.Serialization;

using Regulus.Remote;

namespace RemotingTest
{
	
	public class AutoReleaseTest
	{
		[NUnit.Framework.Test()]
		public void Test()
		{            

			var request = Substitute.For<IGhostRequest>();
            var serializer = Substitute.For<ISerializer>();

            var ghost = new Ghost();

			var ar = new AutoRelease(request, serializer);
			ar.Register(ghost);
			ar.Update();
			ghost = null;
			GC.Collect();
			ar.Update();
			request.Received(1).Request(Arg.Any<ClientToServerOpCode>(), Arg.Any<byte[]>());
		}
	}
}
