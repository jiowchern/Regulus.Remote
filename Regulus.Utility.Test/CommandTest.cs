using NSubstitute;
using Regulus.Remote;
using Regulus.Utility;
using System;
using System.Linq;
using System.Net;
using Regulus.Utility.CommandExtension;

namespace RegulusLibraryTest
{

    public enum TEST_ENUM1
    {
        A, B, C
    };
    public interface IDummy
    {
        void Method1();
        void Method2(int a, int b);

        float Method3(int a, int b);
    }
    public class CommandTest
    {
        /*[Xunit.Fact]
        public void TestCommandRegisterEvent1()
        {
            IDummy dummy = NSubstitute.Substitute.For<IDummy>();
            Command command = new Command();

            command.RegisterEvent += (cmd, ret, args) =>
            {
                Xunit.Assert.Equal("Method3", cmd);
                Xunit.Assert.Equal(typeof(float), ret.Param);
                Xunit.Assert.Equal(typeof(int), args[0].Param);
                Xunit.Assert.Equal(typeof(int), args[1].Param);
            };
            command.RegisterLambda<IDummy, int, int, float>(dummy, (d, i1, i2) => d.Method3(i1, i2), (result) => { });
            command.Run("method3", new[] { "3", "9" });
        }*/
        /*[Xunit.Fact]
        public void TestCommandRegisterEvent2()
        {
            IDummy dummy = NSubstitute.Substitute.For<IDummy>();
            Command command = new Command();

            command.RegisterEvent += (cmd, ret, args) =>
            {
                Xunit.Assert.Equal("Method2", cmd);
                Xunit.Assert.Equal(typeof(void), ret.Param);
                Xunit.Assert.Equal(typeof(int), args[0].Param);
                Xunit.Assert.Equal(typeof(int), args[1].Param);

                Xunit.Assert.Equal("return_Void", ret.Description);
            };
            command.RegisterLambda<IDummy, int, int>(dummy, (d, i1, i2) => d.Method2(i1, i2));
            command.Run("method2", new[] { "3", "9" });
        }*/

        [Xunit.Fact]
        public void TestCommandRegisterEvent3()
        {
            IDummy dummy = NSubstitute.Substitute.For<IDummy>();
            Command command = new Command();
            command.RegisterEvent += (cmd, ret, args) =>
            {
                Xunit.Assert.Equal("m", cmd);
                Xunit.Assert.Equal(typeof(void), ret.Param);
                Xunit.Assert.Equal(typeof(int), args[0].Param);
                Xunit.Assert.Equal(typeof(int), args[1].Param);

                Xunit.Assert.Equal("", ret.Description);
                Xunit.Assert.Equal("l1", args[0].Description);
                Xunit.Assert.Equal("l2", args[1].Description);
            };


            command.Register<int, int>("m [l1 ,l2]", (l1, l2) => { dummy.Method2(l1, l2); });
        }

        [Xunit.Fact]
        public void TestCommandRegisterEvent4()
        {
            IDummy dummy = NSubstitute.Substitute.For<IDummy>();
            Command command = new Command();
            command.RegisterEvent += (cmd, ret, args) =>
            {
                Xunit.Assert.Equal("m", cmd);
                Xunit.Assert.Equal(typeof(float), ret.Param);
                Xunit.Assert.Equal(typeof(int), args[0].Param);
                Xunit.Assert.Equal(typeof(int), args[1].Param);



                Xunit.Assert.Equal("", ret.Description);
                Xunit.Assert.Equal("l1", args[0].Description);
                Xunit.Assert.Equal("l2", args[1].Description);
            };


            command.Register<int, int, float>("m [l1 ,l2]", (l1, l2) => dummy.Method3(l1, l2), f => { });
        }


        /*[Xunit.Fact]
        public void TestCommandLambdaRegister()
        {
            IDummy dummy = NSubstitute.Substitute.For<IDummy>();

            Command command = new Command();

            command.RegisterLambda(dummy, (d) => d.Method1());
            command.Run("method1", new string[] { });
            dummy.Received().Method1();

            command.RegisterLambda<IDummy, int, int>(dummy, (instance, a1, a2) => instance.Method2(a1, a2));
            command.Run("method2", new[] { "1", "2" });
            dummy.Received().Method2(1, 2);

            command.RegisterLambda<IDummy, int, int, float>(dummy, (instance, a1, a2) => instance.Method3(a1, a2), (result) => { });
            command.Run("method3", new[] { "3", "4" });
            dummy.Received().Method3(3, 4);

        }*/

        [Xunit.Fact]
        public void TestCommandAnalysisWithParameters1()
        {
            Command.Analysis analysis = new Command.Analysis("login [ account ,    password, result]");

            Xunit.Assert.Equal("login", analysis.Command);
            Xunit.Assert.Equal("result", analysis.Parameters[2]);
            Xunit.Assert.Equal("account", analysis.Parameters[0]);
            Xunit.Assert.Equal("password", analysis.Parameters[1]);
        }

        [Xunit.Fact]
        public void TestCommandAnalysisWithParameters2()
        {
            Command.Analysis analysis = new Command.Analysis("login [ account ,    password][ result ]");

            Xunit.Assert.Equal("login", analysis.Command);
            Xunit.Assert.Equal("result", analysis.Return);
            Xunit.Assert.Equal("account", analysis.Parameters[0]);
            Xunit.Assert.Equal("password", analysis.Parameters[1]);
        }


        [Xunit.Fact]
        public void TestCommandAnalysisWithParameters3()
        {
            Command.Analysis analysis = new Command.Analysis("login-0.AAA [ account ,    password, result]");

            Xunit.Assert.Equal("login-0.AAA", analysis.Command);
            Xunit.Assert.Equal("result", analysis.Parameters[2]);
            Xunit.Assert.Equal("account", analysis.Parameters[0]);
            Xunit.Assert.Equal("password", analysis.Parameters[1]);
        }

        [Xunit.Fact]
        public void TestCommandAnalysisNoParameters()
        {
            Command.Analysis analysis = new Command.Analysis("login");

            Xunit.Assert.Equal("login", analysis.Command);
            Xunit.Assert.Equal(0, analysis.Parameters.Length);
        }

        [Xunit.Fact]
        public void TestCommandRegister0()
        {
            ICallTester callTester = Substitute.For<ICallTester>();

            Command command = new Command();
            CommandRegister<ICallTester> cr = new CommandRegister<ICallTester>(command, caller => caller.Function1());

            cr.Register(0, callTester);
            command.Run("0Function1", new string[0]);
            callTester.Received(1).Function1();
            cr.Unregister(0);
        }

        [Xunit.Fact]
        public void TestCommandRegister1()
        {
            // data
            Command command = new Command();
            CommandRegister<ICallTester, int> cr = new CommandRegister<ICallTester, int>(

                command,
                (caller, arg1) => caller.Function2(arg1));
            ICallTester callTester = Substitute.For<ICallTester>();

            // test
            cr.Register(0, callTester);
            command.Run(
                "0Function2",
                new[]
                {
                    "1"
                });

            // verify
            cr.Unregister(0);
        }

        [Xunit.Fact]
        public void TestCommandRegister2()
        {
            Command command = new Command();
            CommandRegisterReturn<ICallTester, int> cr = new CommandRegisterReturn<ICallTester, int>(

                command,
                caller => caller.Function3(),
                ret => { });
            ICallTester callTester = Substitute.For<ICallTester>();
            cr.Register(0, callTester);
            command.Run(
                "0Function3",
                new string[]
                {
                });
            callTester.Received(1).Function3();
            cr.Unregister(0);
        }

        [Xunit.Fact]
        public void TestCommandRegister3()
        {
            Command command = new Command();
            int result = 0;
            CommandRegisterReturn<ICallTester, int, byte, float, int> cr = new CommandRegisterReturn<ICallTester, int, byte, float, int>(

                command,
                (caller, arg1, arg2, arg3) => caller.Function4(arg1, arg2, arg3),
                ret => { result = ret; });
            ICallTester callTester = Substitute.For<ICallTester>();
            callTester.Function4(Arg.Any<int>(), Arg.Any<byte>(), Arg.Any<float>()).Returns(1);
            cr.Register(0, callTester);
            command.Run(
                "0Function4",
                new[]
                {
                    "1",
                    "2",
                    "3"
                });
            callTester.Received(1).Function4(Arg.Any<int>(), Arg.Any<byte>(), Arg.Any<float>());
            cr.Unregister(0);
            Xunit.Assert.Equal(1, result);
        }


        [Xunit.Fact]
        public void TestCommandRegister4()
        {
            Command command = new Command();
            int result = 0;
            CommandRegisterReturn<ICallTester, int> cr = new CommandRegisterReturn<ICallTester, int>(

                command,
                (caller) => caller.Function5,
                ret => { result = ret; });
            ICallTester callTester = Substitute.For<ICallTester>();
            callTester.Function5.Returns(1);
            cr.Register(0, callTester);
            command.Run(
                "0Function5",
                new string[0]);

            cr.Unregister(0);

            Xunit.Assert.Equal(1, result);
        }
        [Xunit.Fact]
        public void TestCommandIpEndPointEnum()
        {
            object outVal;
            Command.TryConversion("127.0.0.1:12345", out outVal, typeof(System.Net.IPEndPoint));
            IPEndPoint ip = outVal as System.Net.IPEndPoint;
            Xunit.Assert.Equal(ip.Address, IPAddress.Parse("127.0.0.1"));
            Xunit.Assert.Equal(ip.Port, 12345);
        }

        [Xunit.Fact]
        public void TestCommandGuid()
        {
            Guid id = Guid.NewGuid();
            object outVal;
            Command.TryConversion(id.ToString(), out outVal, typeof(Guid));
            Guid id2 = (Guid)outVal;
            Xunit.Assert.Equal(id.ToString(), id2.ToString());
        }
        [Xunit.Fact]
        public void TestCommandCnvEnum()
        {
            object outVal;
            Command.TryConversion("A", out outVal, typeof(TEST_ENUM1));

            Xunit.Assert.Equal(TEST_ENUM1.A, outVal);

            Action<TEST_ENUM1> action = _CallEnum;

            action.Invoke((TEST_ENUM1)outVal);


        }

        private void _CallEnum(TEST_ENUM1 obj)
        {

        }
        [Xunit.Fact]
        public void UnanalyzableCommand()
        {
            var handler = new UnanalyzableCommandHandler();
            
            Command command = new Command();
            command.Register("Unanalyzable.Command.TestRun.Run", handler.Run );
            command.Run("Unanalyzable.Command.TestRun.Run" , new string[0]);
            Xunit.Assert.Equal(true , handler.Called );

        }
        [Xunit.Fact]
        public void CommandReturnTest()
        {
            Command command = new Command();
            command.Register<int,int,int>("add" , _AddTest  );
            var rets = command.Run("add" , new[] { "1", "1" });
            var val = (int)rets.First();
            Xunit.Assert.Equal(2 , val);
        }

        private int _AddTest(int arg1, int arg2)
        {
            return arg1 + arg2;
        }
    }

}
