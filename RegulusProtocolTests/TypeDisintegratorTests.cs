using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regulus.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regulus.Protocol.Tests
{
    [TestClass()]
    public class TypeDisintegratorTests
    {
        [TestMethod()]
        public void TypeDisintegratorTest()
        {
            var typeDisintegrator = new TypeDisintegrator(typeof(int));
        }
    }
}