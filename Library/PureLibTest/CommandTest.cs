using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSubstitute;


using Regulus.Remoting;
using Regulus.Utility;

namespace RegulusLibraryTest
{
    public interface IDummy
    {
        void Method1();
        void Method2(int a , int b);

        float Method3(int a, int b);
    }
	[TestClass]
	public class CommandTest
	{
	    [TestMethod]
	    public void TestCommandRegisterEvent1()
	    {
            var dummy = NSubstitute.Substitute.For<IDummy>();
            var command = new Command();

	        command.RegisterEvent += (cmd, ret, args) =>
	        {
	            Assert.AreEqual("Method3" , cmd);
                Assert.AreEqual(typeof(float), ret.Param);
                Assert.AreEqual(typeof(int), args[0].Param);
                Assert.AreEqual(typeof(int), args[1].Param);
            };
            command.RegisterLambda<IDummy, int, int, float>(dummy, (d,i1,i2) => d.Method3(i1,i2), (result) => { });
            command.Run("method3", new [] { "3" , "9"});
        }
        [TestMethod]
        public void TestCommandRegisterEvent2()
        {
            var dummy = NSubstitute.Substitute.For<IDummy>();
            var command = new Command();

            command.RegisterEvent += (cmd, ret, args) =>
            {
                Assert.AreEqual("Method2", cmd);
                Assert.AreEqual(typeof(void), ret.Param);
                Assert.AreEqual(typeof(int), args[0].Param);
                Assert.AreEqual(typeof(int), args[1].Param);

                Assert.AreEqual("return_Void" ,ret.Description );
            };
            command.RegisterLambda<IDummy, int, int>(dummy, (d, i1, i2) => d.Method2(i1, i2));
            command.Run("method2", new[] { "3", "9" });
        }

	    [TestMethod]
	    public void TestCommandRegisterEvent3()
	    {
            var dummy = NSubstitute.Substitute.For<IDummy>();
            var command = new Command();
            command.RegisterEvent += (cmd, ret, args) =>
            {
                Assert.AreEqual("m", cmd);
                Assert.AreEqual(typeof(void), ret.Param);
                Assert.AreEqual(typeof(int), args[0].Param);
                Assert.AreEqual(typeof(int), args[1].Param);

                Assert.AreEqual("", ret.Description);
                Assert.AreEqual("l1", args[0].Description);
                Assert.AreEqual("l2", args[1].Description);
            };


            command.Register<int,int>("m [l1 ,l2]" , (l1, l2) => { dummy.Method2(l1,l2);});
        }

        [TestMethod]
        public void TestCommandRegisterEvent4()
        {
            var dummy = NSubstitute.Substitute.For<IDummy>();
            var command = new Command();
            command.RegisterEvent += (cmd, ret, args) =>
            {
                Assert.AreEqual("m", cmd);
                Assert.AreEqual(typeof(float), ret.Param);
                Assert.AreEqual(typeof(int), args[0].Param);
                Assert.AreEqual(typeof(int), args[1].Param);



                Assert.AreEqual("", ret.Description);
                Assert.AreEqual("l1", args[0].Description);
                Assert.AreEqual("l2", args[1].Description);
            };


            command.Register<int, int , float>("m [l1 ,l2]", (l1, l2) => dummy.Method3(l1, l2) , f => {});
        }


        [TestMethod]
	    public void TestCommandLambdaRegister()
	    {
            var dummy = NSubstitute.Substitute.For<IDummy>();

            var command = new Command();

            command.RegisterLambda(dummy , (d) =>  d.Method1() );
	        command.Run("method1", new string[]{});
            dummy.Received().Method1();

            command.RegisterLambda<IDummy , int,int>(dummy, (instance, a1 , a2) => instance.Method2(a1,a2));
            command.Run("method2", new [] { "1" , "2"});            
            dummy.Received().Method2( 1 , 2);

            command.RegisterLambda<IDummy, int, int , float>(dummy, (instance, a1, a2) => instance.Method3(a1, a2) , (result) => {});
            command.Run("method3", new[] { "3", "4" });
            dummy.Received().Method3(3, 4);

        }

        [TestMethod]
		public void TestCommandAnalysisWithParameters1()
		{
			var analysis = new Command.Analysis("login [ account ,    password, result]");

			Assert.AreEqual("login", analysis.Command);
			Assert.AreEqual("result", analysis.Parameters[2]);
			Assert.AreEqual("account", analysis.Parameters[0]);
			Assert.AreEqual("password", analysis.Parameters[1]);
		}

        [TestMethod]
        public void TestCommandAnalysisWithParameters2()
        {
            var analysis = new Command.Analysis("login [ account ,    password][ result ]");

            Assert.AreEqual("login", analysis.Command);
            Assert.AreEqual("result", analysis.Return);
            Assert.AreEqual("account", analysis.Parameters[0]);
            Assert.AreEqual("password", analysis.Parameters[1]);
        }

        [TestMethod]
		public void TestCommandAnalysisNoParameters()
		{
			var analysis = new Command.Analysis("login");

			Assert.AreEqual("login", analysis.Command);
			Assert.AreEqual(0, analysis.Parameters.Length);
		}

		[TestMethod]
		public void TestCommandRegister0()
		{
			var command = new Command();
			var cr = new CommandRegister<ICallTester>(command, caller => caller.Function1());
			var callTester = Substitute.For<ICallTester>();
			cr.Register(callTester);
			command.Run("Function1", new string[0]);
			callTester.Received(1).Function1();
			cr.Unregister();
		}

		[TestMethod]
		public void TestCommandRegister1()
		{
			// data
			var command = new Command();
			var cr = new CommandRegister<ICallTester, int>(
				
				command, 
				(caller, arg1) => caller.Function2(arg1));
			var callTester = Substitute.For<ICallTester>();

			// test
			cr.Register(callTester);
			command.Run(
				"Function2", 
				new[]
				{
					"1"
				});

			// verify
			cr.Unregister();
		}

		[TestMethod]
		public void TestCommandRegister2()
		{
			var command = new Command();
			var cr = new CommandRegisterReturn<ICallTester, int>(
				
				command, 
				caller => caller.Function3(), 
				ret => { });
			var callTester = Substitute.For<ICallTester>();
			cr.Register(callTester);
			command.Run(
				"Function3", 
				new string[]
				{
				});
			callTester.Received(1).Function3();
			cr.Unregister();
		}

		[TestMethod]
		public void TestCommandRegister3()
		{
			var command = new Command();
			var cr = new CommandRegisterReturn<ICallTester, int, byte, float, int>(
				
				command, 
				(caller, arg1, arg2, arg3) => caller.Function4(arg1, arg2, arg3), 
				ret => { });
			var callTester = Substitute.For<ICallTester>();
			cr.Register(callTester);
			command.Run(
				"Function4", 
				new[]
				{
					"1", 
					"2", 
					"3"
				});
			callTester.Received(1).Function4(Arg.Any<int>(), Arg.Any<byte>(), Arg.Any<float>());
			cr.Unregister();
		}


        [TestMethod]
        public void TestCommandRegister4()
        {
            var command = new Command();
            int result = 0;
            var cr = new CommandRegisterReturn<ICallTester,  int>(

                command,
                (caller) => caller.Function5,
                ret => { result = ret; });
            var callTester = Substitute.For<ICallTester>();
            callTester.Function5.Returns(1);
            cr.Register(callTester);
            command.Run(
                "Function5",
                new string[0]);
            
            cr.Unregister();

            Assert.AreEqual(1 , result);
        }
    }
}
