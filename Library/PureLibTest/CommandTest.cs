using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using PureLibTest;

namespace Regulus.UnitTest
{
   
    
    [TestClass]
    public class CommandTest
    {

        [TestMethod]
        public void TestCommandAnalysisWithParameters()
        {
            var analysis = new Regulus.Utility.Command.Analysis("login [ account ,    password, result]");            

            Assert.AreEqual("login", analysis.Command);
            Assert.AreEqual("result", analysis.Parameters[2]);
            Assert.AreEqual("account", analysis.Parameters[0]);
            Assert.AreEqual("password", analysis.Parameters[1]);
            
        }


        [TestMethod]
        public void TestCommandAnalysisNoParameters()
        {
            var analysis = new Regulus.Utility.Command.Analysis("login");

            Assert.AreEqual("login", analysis.Command);
            Assert.AreEqual(0, analysis.Parameters.Length);

        }


        [TestMethod]
        public void TestCommandRegister0()
        {
            var command = new Regulus.Utility.Command();
            var cr = new Regulus.Remoting.CommandRegister<ICallTester>("Function1", new string[0], command, (caller) => caller.Function1());
            var callTester = NSubstitute.Substitute.For<ICallTester>();            
            cr.Register(callTester);
            command.Run("Function1", new string[0]);
            callTester.Received(1).Function1();
            cr.Unregister();
        }

        [TestMethod]
        public void TestCommandRegister1()
        {
            var command = new Regulus.Utility.Command();
            var cr = new Regulus.Remoting.CommandRegister<ICallTester, int>("Function2", new string[] { "arg1" }, command, (caller, arg1) => caller.Function2(arg1));
            var callTester = NSubstitute.Substitute.For<ICallTester>();
            cr.Register(callTester);
            command.Run("Function2", new string[] {"1"} );
            callTester.Received(1).Function2( NSubstitute.Arg.Any<int>() );
            cr.Unregister();
        }

        [TestMethod]
        public void TestCommandRegister2()
        {
            var command = new Regulus.Utility.Command();
            var cr = new Regulus.Remoting.CommandRegisterReturn<ICallTester, int>("Function3", new string[] { }, command, (caller) => caller.Function3(), (ret) => { });
            var callTester = NSubstitute.Substitute.For<ICallTester>();
            cr.Register(callTester);
            command.Run("Function3", new string[] {  });
            callTester.Received(1).Function3();
            cr.Unregister();
        }


        [TestMethod]
        public void TestCommandRegister3()
        {
            var command = new Regulus.Utility.Command();
            var cr = new Regulus.Remoting.CommandRegisterReturn<ICallTester, int , byte , float, int>("Function4", new string[] { }, command, (caller,arg1,arg2,arg3) => caller.Function4(arg1,arg2,arg3), (ret) => { });
            var callTester = NSubstitute.Substitute.For<ICallTester>();
            cr.Register(callTester);
            command.Run("Function4", new string[] { "1", "2" , "3" });
            callTester.Received(1).Function4(NSubstitute.Arg.Any<int>(), NSubstitute.Arg.Any<byte>(), NSubstitute.Arg.Any<float>());
            cr.Unregister();
        }

        
    }
}
