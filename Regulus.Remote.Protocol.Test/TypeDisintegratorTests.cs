
using Regulus.Remote.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Regulus.Remote.Protocol.Tests
{
    public class TypeDisintegratorTests
    {
        [NUnit.Framework.Test()]
        public void TypeDisintegratorPointerTest()
        {
            var type = Type.GetType("System.Void*");
            var typeDisintegrator = new TypeDisintegrator(type);
            NUnit.Framework.Assert.AreEqual(0, typeDisintegrator.Types.Length);
        }

        [NUnit.Framework.Test()]
        public void TypeDisintegratorIntTest()
        {
            var typeDisintegrator = new TypeDisintegrator(typeof(int));
            NUnit.Framework.Assert.AreEqual(typeof(int) , typeDisintegrator.Types[0]);            
        }


        [NUnit.Framework.Test()]
        public void TypeDisintegratorInterfaceTest()
        {
            var typeDisintegrator = new TypeDisintegrator(typeof(ITest ));
            NUnit.Framework.Assert.IsTrue(typeDisintegrator.Types.Any(t=> t == typeof(Guid)));
            NUnit.Framework.Assert.IsTrue(typeDisintegrator.Types.Any(t => t == typeof(string)));
            NUnit.Framework.Assert.IsTrue(typeDisintegrator.Types.Any(t => t == typeof(float)));
            NUnit.Framework.Assert.IsTrue(typeDisintegrator.Types.Any(t => t == typeof(byte)));
            NUnit.Framework.Assert.IsTrue(typeDisintegrator.Types.Any(t => t == typeof(int)));
            NUnit.Framework.Assert.IsTrue(typeDisintegrator.Types.Any(t => t == typeof(EventData1)));
            NUnit.Framework.Assert.IsTrue(typeDisintegrator.Types.Any(t => t == typeof(EventData2)));
        }


        [NUnit.Framework.Test()]
        public void TypeDisintegratorClassTest()
        {
            var typeDisintegrator = new TypeDisintegrator(typeof(EventData1));            
            NUnit.Framework.Assert.IsTrue(typeDisintegrator.Types.Any(t => t == typeof(EventData1)));
            NUnit.Framework.Assert.IsTrue(typeDisintegrator.Types.Any(t => t == typeof(EventData2)));
        }


        [NUnit.Framework.Test()]
        [NUnit.Framework.MaxTime(1000)]
        public void TypeDisintegratorNestTest()
        {
            var typeDisintegrator = new TypeDisintegrator(typeof(Data1));
            NUnit.Framework.Assert.IsTrue(typeDisintegrator.Types.Any(t => t == typeof(Data1)));
            NUnit.Framework.Assert.IsTrue(typeDisintegrator.Types.Any(t => t == typeof(Data2)));
            NUnit.Framework.Assert.IsTrue(typeDisintegrator.Types.Any(t => t == typeof(Data1[])));
            NUnit.Framework.Assert.IsTrue(typeDisintegrator.Types.Any(t => t == typeof(Data2[])));
        }
    }

    public interface ITest
    {
        Guid Id { get; }
        event Action<float> Event1;
        event Action<EventData1> Event2;
        Regulus.Remote.Value<byte> Method1(int arg1, string arg2);
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

    public class Data1
    {
        public Data2[] Data2s;
    }

    public class Data2
    {
        public Data1[] Data1s;
    }
}



