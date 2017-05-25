/*
using Regulus.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
namespace Regulus.Serialization.Tests
{
    
    public class SerializerTests
    {
        [NUnit.Framework.Test()]
        public void SerializeTest1()
        {
            var provider = NSubstitute.Substitute.For<ITypeProvider>();
            var serializer = new Serializer(new [] { provider });
            
            var buffer = serializer.Serialize(0);
            var val = serializer.Deserialize<int>(buffer);
            NUnit.Framework.Assert.AreEqual(0 , val);
        }

        [NUnit.Framework.Test()]
        public void DeserializeTest1()
        {
            var provider = new IntTypeProvider();
            var serializer = new Serializer(new[] { provider });

            var buffer = new byte[]
            {
                1,0,0,0 , 1,0,0,0, 0,0,0,0, 1,0,0,0, // head
                1,0,0,0 , 1,0,0,0, // type info                
                0,0,0,0 , 0,0,0,0, 1,0,0,0,// ref info                
                1,0,0,0 , 0,0,0,0, // value info
                100,0,0,0             //data

            };
            var val = serializer.Deserialize<int>(buffer);
            NUnit.Framework.Assert.AreEqual(100 , val);
        }
    }

    public class IntTypeProvider : ITypeProvider
    {
        int ITypeProvider.Id
        {
            get { return 1; }
        }

        Type ITypeProvider.Type
        {
            get { return typeof (int); }
        }

        object ITypeProvider.CreateInstnace()
        {
            return new int();
        }

        object ITypeProvider.CreateInstnace(byte[] buffer, int index)
        {
            var val = BitConverter.ToInt32(buffer, index);
            return val;
        }

        FieldInfo ITypeProvider.GetField(int field)
        {
            throw new NotImplementedException();
        }

        bool ITypeProvider.IsValueType()
        {
            return true;
        }
    }
}
*/