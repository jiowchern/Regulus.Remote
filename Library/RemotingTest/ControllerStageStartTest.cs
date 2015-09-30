using System.Security.Cryptography;


using Microsoft.VisualStudio.TestTools.UnitTesting;


using Regulus.Utility;

namespace RemotingTest
{
    [TestClass]
    public class ControllerStageStartTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            bool pass = false;
            var view = NSubstitute.Substitute.For<Console.IViewer>();
            var command = new Command();
            command.Register("a" ,
                (string p1, string p2, string p3) =>
                {
                    pass = p1 == "1" && p2 == "2" && p3 == "3";
                });
            IStage stage = new Regulus.Remoting.Soul.Native.StageStart(command , view , new [] {"a" , "1" , "2" , "3" });
            stage.Enter();
            stage.Update();
            stage.Leave();

            Assert.IsTrue(pass);
        }
    }
}
