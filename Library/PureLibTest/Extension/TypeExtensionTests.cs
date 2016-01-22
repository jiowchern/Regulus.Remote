using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regulus.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Regulus.CustomType;
using Regulus.Extension;
namespace Regulus.Extension.Tests
{
    [TestClass()]
    public class TypeExtensionTests
    {
        [TestMethod()]
        public void FindHullTest()
        {
            Vector2[] points = new Vector2[]
            {
                new Vector2(497.5674f , 125.8552f),
                new Vector2(497.5874f , 125.3556f),
                new Vector2(498.0869f , 125.3756f),
                new Vector2(498.0669f , 125.8752f),

                new Vector2(497.5667f , 125.8711f),
                new Vector2(497.5867f , 125.3716f),
                new Vector2(498.0863f , 125.3916f),
                new Vector2(498.0663f , 125.8912f)
            };

            points.FindHull().ToArray();
        }
    }
}