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
        public void FindHullTest1()
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
            for (int i = 0; i < 1000000; i++)
            {
                points.FindHull().ToArray();
            }
            
        }
        [TestMethod()]
        public void FindHullTest2()
        {
            Vector2[] points = new Vector2[]
            {
                new Vector2(303.4322f , 68.75738f),
                new Vector2(303.4522f , 68.25784f),
                new Vector2(303.9518f , 68.27787f),
                new Vector2(303.9318f ,  68.77739f),

                new Vector2(303.4328f , 68.74139f),
                new Vector2(303.4529f ,  68.24185f),
                new Vector2(303.9524f , 68.26188f),
                new Vector2(303.9324f , 68.7614f)
            };
            for (int i = 0; i < 1000000; i++)
            {
                points.FindHull().ToArray();
            }

        }

        /*
        ptps = 
x : 303.4529, y : 68.24185, K : -24.95236, Distance : 0.2661937,
x : 303.4522, y : 68.25784, K : -24.95236, Distance : 0.2499356,
x : 303.9524, y : 68.26188, K : -0.952455, Distance : 0.5161614,
x : 303.9518, y : 68.27787, K : -0.9228533, Distance : 0.4999035,
x : 303.9324, y : 68.7614, K : 0.008037948, Distance : 0.2502298,
x : 303.9318, y : 68.77739, K : 0.04005803, Distance : 0.2499734 
points 
X : 303.4322, Y : 68.75738, Magnitude : 311.1248,
X : 303.4522, Y : 68.25784, Magnitude : 311.0344,
X : 303.9518, Y : 68.27787, Magnitude : 311.5262,
X : 303.9318, Y : 68.77739, Magnitude : 311.6165,

X : 303.4328, Y : 68.74139, Magnitude : 311.1219,
X : 303.4529, Y : 68.24185, Magnitude : 311.0315,
X : 303.9524, Y : 68.26188, Magnitude : 311.5233,
X : 303.9324, Y : 68.7614, Magnitude : 311.6136 
curs 
x : 303.4322, y : 68.75738, K : 0, Distance : 0
        */
    }
}