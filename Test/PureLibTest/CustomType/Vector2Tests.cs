using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regulus.CustomType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regulus.CustomType.Tests
{
    [TestClass()]
    public class Vector2Tests
    {
        [TestMethod()]
        public void VectorToAngleTest()
        {
            var vec = Vector2.AngleToVector(45.0f);
            var angle = Vector2.VectorToAngle(vec);
            Assert.AreEqual(45.0f , angle);
        }
    }
}