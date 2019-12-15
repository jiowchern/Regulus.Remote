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

            var ar = new AutoRelease(request, serializer);
            _Register(ar);
            ar.Update();

            GC.Collect();
            GC.WaitForFullGCComplete();
            GC.WaitForPendingFinalizers();

            ar.Update();
            request.Received(1).Request(Arg.Any<ClientToServerOpCode>(), Arg.Any<byte[]>());
        }

        private static AutoRelease _Register(AutoRelease ar)
        {
            var ghost = new Ghost();
            ar.Register(ghost);
            return ar;
        }
    }
}
