using System;
using System.Linq;
using Xunit;


namespace Regulus.Remote.Protocol.Tests
{
    public class TypeDisintegratorTests
    {
        [Xunit.Fact]
        public void TypeDisintegratorPointerTest()
        {
            Type type = Type.GetType("System.Void*");
            TypeDisintegrator typeDisintegrator = new TypeDisintegrator(type);
            
            Xunit.Assert.Empty(typeDisintegrator.Types);
        }

        [Xunit.Fact]
        public void TypeDisintegratorIntTest()
        {
            TypeDisintegrator typeDisintegrator = new TypeDisintegrator(typeof(int));
            Xunit.Assert.Equal(typeof(int), typeDisintegrator.Types[0]);
        }


        [Xunit.Fact]
        public void TypeDisintegratorInterfaceTest()
        {
            TypeDisintegrator typeDisintegrator = new TypeDisintegrator(typeof(ITest));
            Xunit.Assert.Contains(typeDisintegrator.Types, t => t == typeof(Guid));
            Xunit.Assert.Contains(typeDisintegrator.Types, t => t == typeof(string));
            Xunit.Assert.Contains(typeDisintegrator.Types, t => t == typeof(float));
            Xunit.Assert.Contains(typeDisintegrator.Types, t => t == typeof(byte));
            Xunit.Assert.Contains(typeDisintegrator.Types, t => t == typeof(int));
            Xunit.Assert.Contains(typeDisintegrator.Types, t => t == typeof(EventData1));
            Xunit.Assert.Contains(typeDisintegrator.Types, t => t == typeof(EventData2));
        }


        [Xunit.Fact]
        public void TypeDisintegratorClassTest()
        {
            TypeDisintegrator typeDisintegrator = new TypeDisintegrator(typeof(EventData1));
            Xunit.Assert.Contains(typeDisintegrator.Types, t => t == typeof(EventData1));
            Xunit.Assert.Contains(typeDisintegrator.Types, t => t == typeof(EventData2));
        }


        [Fact(Timeout =1000)]        
        public void TypeDisintegratorNestTest()
        {
            TypeDisintegrator typeDisintegrator = new TypeDisintegrator(typeof(Data1));
            Xunit.Assert.Contains(typeDisintegrator.Types, t => t == typeof(Data1));
            Xunit.Assert.Contains(typeDisintegrator.Types, t => t == typeof(Data2));
            Xunit.Assert.Contains(typeDisintegrator.Types, t => t == typeof(Data1[]));
            Xunit.Assert.Contains(typeDisintegrator.Types, t => t == typeof(Data2[]));
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



