using NSubstitute;
using Regulus.Remote;
using Regulus.Remote.Extension;
using Regulus.Utility;
using System;

namespace RemotingTest
{

    public class CommandCall
    {
        [NUnit.Framework.Test()]
        public void TestCommandCall()
        {
            CommandParam param = Substitute.For<CommandParam>();
            bool called = false;
            param.Types = new[]
            {
                typeof(string)
            };
            param.Callback = new Action<string>(msg => { called = true; });

            Command command = new Command();
            command.Register("123", param);
            command.Run(
                "123",
                new[]
                {
                    " Hello World."
                });

            NUnit.Framework.Assert.AreEqual(true, called);
        }

        [NUnit.Framework.Test()]
        public void TestCommandAdd()
        {
            CommandParam param = Substitute.For<CommandParam>();
            float value = 0;
            param.Types = new[]
            {
                typeof(int),
                typeof(int)
            };
            param.ReturnType = typeof(float);

            param.Callback = new Func<int, int, float>((a, b) => { return a + b; });
            param.Return = new Action<float>(val => { value = val; });

            Command command = new Command();
            command.Register("123", param);
            command.Run(
                "123",
                new[]
                {
                    "1",
                    "2"
                });

            NUnit.Framework.Assert.AreEqual(3, value);
        }

        [NUnit.Framework.Test()]
        public void TestGPIBinder()
        {
            Command command = new Command();
            IBinderTest tester = Substitute.For<IBinderTest>();
            TestNotifier notifier = new TestNotifier(tester);


            GPIBinder<IBinderTest> binder = new GPIBinder<IBinderTest>(notifier, command);
            IBootable boot = binder;
            boot.Launch();


            binder.Bind(t => t.Function1());
            binder.Bind<int>((t, arg) => t.Function2(arg));
            bool returnValue = false;
            int returnProperty = 0;
            binder.Bind(t => t.Function3(), ret => { returnValue = true; });

            binder.Bind(t => t.Property1, ret => { returnProperty = 12345; });

            notifier.InvokeSupply();

            command.Run("0Function1", new string[0]);
            command.Run("0Function2", new string[] { "10" });
            command.Run("0Function3", new string[0]);
            command.Run("0Property1", new string[0]);




            boot.Shutdown();

            tester.Received().Function1();
            tester.Received().Function2(Arg.Any<int>());

            NUnit.Framework.Assert.AreEqual(true, returnValue);
            NUnit.Framework.Assert.AreEqual(12345, returnProperty);
        }


        class TestNotifier : INotifier<IBinderTest>
        {
            private readonly IBinderTest _Tester;

            private event Action<IBinderTest> _Supply;
            public TestNotifier(IBinderTest tester)
            {
                _Tester = tester;

            }

            public void InvokeSupply()
            {
                _Supply(_Tester);
            }


            event Action<IBinderTest> INotifier<IBinderTest>.Supply
            {
                add { _Supply += value; }
                remove { _Supply -= value; }
            }

            event Action<IBinderTest> INotifier<IBinderTest>.Unsupply
            {
                add { }
                remove { }
            }

            IBinderTest[] INotifier<IBinderTest>.Ghosts
            {
                get
                {
                    return new[]
                    {
                        _Tester
                    };
                }
            }


        }
    }
}
