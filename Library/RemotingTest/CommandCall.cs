using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RemotingTest
{
    using NSubstitute;
    using Regulus.Remoting.Extension;
    [TestClass]
    public class CommandCall
    {
        [TestMethod]
        public void TestCommandCall()
        {
            
            var param = NSubstitute.Substitute.For<Regulus.Remoting.CommandParam>();
            bool called = false;                        
            param.Types = new Type[] { typeof(string) };
            param.Callback = new Action<string>((msg) => { called = true; });
            

            Regulus.Utility.Command command = new Regulus.Utility.Command();            
            command.Register("123", param);
            command.Run("123", new string[] {" Hello World."} );


            Assert.AreEqual(true, called);
        }

        [TestMethod]
        public void TestCommandAdd()
        {
            var param = NSubstitute.Substitute.For<Regulus.Remoting.CommandParam>();
            float value = 0;
            param.Types = new Type[] { typeof(int), typeof(int) };
            param.ReturnType = typeof(float);

            param.Callback = new Func<int, int , float>((a, b) => { return a + b; });
            param.Return = new Action<float>((val) => { value = val;  });

            Regulus.Utility.Command command = new Regulus.Utility.Command();
            command.Register("123", param);
            command.Run("123", new string[] { "1" , "2" });

            Assert.AreEqual(3, value);
        }



        [TestMethod]
        public void TestGPIBinder()
        {
            var command =new  Regulus.Utility.Command();
            var notifier = NSubstitute.Substitute.For<Regulus.Remoting.Ghost.INotifier<IBinderTest>>();

            var binder = new Regulus.Remoting.GPIBinder<IBinderTest>(notifier, command);
            
            binder.Bind((tester) => tester.Function1() );

            binder.Bind<int>((tester, arg) => tester.Function2(arg));

            binder.Bind<int>((tester) => tester.Function3(), (ret) => { });
        }


        
        
    }
}
