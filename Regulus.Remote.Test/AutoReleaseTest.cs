using NSubstitute;
using Regulus.Remote;
using Regulus.Serialization;
using System;

namespace RemotingTest
{

    public class AutoReleaseTest
    {
        [NUnit.Framework.Test]
        public void Test()
        {

            IGhostRequest request = Substitute.For<IGhostRequest>();
            var serializer = Substitute.For<IInternalSerializable>();

            AutoRelease ar = new AutoRelease(request, serializer);
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
            Ghost ghost = new Ghost();
            ar.Register(ghost);
            return ar;
        }
    }
}
