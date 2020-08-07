using NUnit.Framework;

namespace Regulus.Remote.Protocol.Tests
{
    interface IInterface
    {
        void Method1();
        void Method2(int a, int b);

        Regulus.Remote.Value<bool> Method3(int a, int b);
    }
    [TestFixture()]
    public class CodeBuilderTests
    {
        [Test()]
        public void _BuildGetTypeMethodInfoTestMethod1()
        {
            CodeBuilder builder = new Regulus.Remote.Protocol.CodeBuilder();
            string source = "new Regulus.Utility.Reflection.TypeMethodCatcher((System.Linq.Expressions.Expression<System.Action<Regulus.Remote.Protocol.Tests.IInterface>>)((ins) => ins.Method1())).Method";
            string code = builder._BuildGetTypeMethodInfo(
                typeof(IInterface).GetMethod(nameof(IInterface.Method1)));
            Assert.AreEqual(source, code);
        }

        [Test()]
        public void _BuildGetTypeMethodInfoTestMethod2()
        {
            CodeBuilder builder = new Regulus.Remote.Protocol.CodeBuilder();
            string source = "new Regulus.Utility.Reflection.TypeMethodCatcher((System.Linq.Expressions.Expression<System.Action<Regulus.Remote.Protocol.Tests.IInterface,System.Int32,System.Int32>>)((ins,_1,_2) => ins.Method2(_1,_2))).Method";
            string code = builder._BuildGetTypeMethodInfo(
                typeof(IInterface).GetMethod(nameof(IInterface.Method2)));
            Assert.AreEqual(source, code);
        }

        [Test()]
        public void _BuildGetTypeMethodInfoTestMethod3()
        {
            CodeBuilder builder = new Regulus.Remote.Protocol.CodeBuilder();
            string source = "new Regulus.Utility.Reflection.TypeMethodCatcher((System.Linq.Expressions.Expression<System.Action<Regulus.Remote.Protocol.Tests.IInterface,System.Int32,System.Int32>>)((ins,_1,_2) => ins.Method3(_1,_2))).Method";
            string code = builder._BuildGetTypeMethodInfo(
                typeof(IInterface).GetMethod(nameof(IInterface.Method3)));
            Assert.AreEqual(source, code);
        }
    }
}