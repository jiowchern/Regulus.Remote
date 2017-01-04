using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regulus.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regulus.Serialization.Tests
{
    [TestClass()]
    public class SerializerTests
    {
        [TestMethod()]
        public void SerializeTest()
        {
            var serializer = new Serializer(new[] { typeof(int) });
            
            var buffer = serializer.Serialize<int>(0);
        }
    }
}