using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regulus.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Regulus.Protocol.Tests
{
    [TestClass()]
    public class TypeDisintegratorTests
    {
        [TestMethod()]
        public void TypeDisintegratorIntTest()
        {
            var typeDisintegrator = new TypeDisintegrator(typeof(int));
            Assert.AreEqual(typeof(int) , typeDisintegrator.Types[0]);            
        }


        [TestMethod()]
        public void TypeDisintegratorInterfaceTest()
        {
            var typeDisintegrator = new TypeDisintegrator(typeof(ITest ));
            Assert.IsTrue(typeDisintegrator.Types.Any(t=> t == typeof(Guid)));
            Assert.IsTrue(typeDisintegrator.Types.Any(t => t == typeof(string)));
            Assert.IsTrue(typeDisintegrator.Types.Any(t => t == typeof(float)));
            Assert.IsTrue(typeDisintegrator.Types.Any(t => t == typeof(byte)));
            Assert.IsTrue(typeDisintegrator.Types.Any(t => t == typeof(int)));
            Assert.IsTrue(typeDisintegrator.Types.Any(t => t == typeof(EventData1)));
            Assert.IsTrue(typeDisintegrator.Types.Any(t => t == typeof(EventData2)));
        }


        [TestMethod()]
        public void TypeDisintegratorClassTest()
        {
            var typeDisintegrator = new TypeDisintegrator(typeof(EventData1));            
            Assert.IsTrue(typeDisintegrator.Types.Any(t => t == typeof(EventData1)));
            Assert.IsTrue(typeDisintegrator.Types.Any(t => t == typeof(EventData2)));
        }
    }

    public interface ITest
    {
        Guid Id { get; }
        event Action<float> Event1;
        event Action<EventData1> Event2;
        Regulus.Remoting.Value<byte> Method1(int arg1, string arg2);
        void Method2(int arg1, EventData1 arg2);

    }

    public class EventData2
    {
        public string Data1;
    }
    public class EventData1
    {
        public EventData2 Data1;
        public int Data2;
    }
}



