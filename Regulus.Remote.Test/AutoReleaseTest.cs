using NSubstitute;
using Regulus.Remote;
using Regulus.Serialization;
using System;

namespace RemotingTest
{

    public class AutoReleaseTest
    {
        [Xunit.Fact]
        public void Test()
        {

            IGhostRequest request = Substitute.For<IGhostRequest>();
            ISerializable serializer = Substitute.For<ISerializable>();

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
