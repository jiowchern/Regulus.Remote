using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
namespace RemotingTest
{
    [TestClass]
    public class AutoReleaseTest
    {
        
        [TestMethod]
        public void Test()
        {
            var request = NSubstitute.Substitute.For<Regulus.Remoting.IGhostRequest>();
            
            var ghost = new Ghost();


            Regulus.Remoting.AutoRelease ar = new Regulus.Remoting.AutoRelease(request);
            ar.Register(ghost);
            ar.Update();
            ghost = null;
            System.GC.Collect();
            ar.Update();
            request.Received(1).Request(NSubstitute.Arg.Any<byte>(), NSubstitute.Arg.Any<System.Collections.Generic.Dictionary<byte, byte[]>>());
        }
    }
}
