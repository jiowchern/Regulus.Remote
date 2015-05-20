using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PureLibTest
{

    enum TESTENUM
    {
        _1, _2, _3
    };

    [TestClass]
    public class FlagTest
    {
        [TestMethod]
        public void TestToArray()
        {
            var flags = new Regulus.CustomType.Flag<TESTENUM>();
            flags[TESTENUM._1] = true;
            flags[TESTENUM._2] = false;
            flags[TESTENUM._3] = true;

            var array = flags.ToArray();

            Assert.AreNotEqual(TESTENUM._2, array[0]);
            Assert.AreNotEqual(TESTENUM._2, array[1]);
        }
        [TestMethod]
        public void TestCustomFlag1()
        {
            var flags = new Regulus.CustomType.Flag<TESTENUM>();
            flags[TESTENUM._1] = false;
            flags[TESTENUM._2] = true;
            flags[TESTENUM._3] = false;


            Assert.AreEqual(true, flags[TESTENUM._2]);
            Assert.AreEqual(false, flags[TESTENUM._3]);
        }

        [TestMethod]
        public void TestCustomFlag2()
        {
            var flags = new Regulus.CustomType.Flag<TESTENUM>(new[] { TESTENUM._1, TESTENUM._3 });            


            Assert.AreEqual(false, flags[TESTENUM._2]);
            Assert.AreEqual(true, flags[TESTENUM._3]);
        }

        [TestMethod]
        public void TestCustomFlagSerializer()
        {
            var flags = new Regulus.CustomType.Flag<TESTENUM>(new[] { TESTENUM._1, TESTENUM._3 });


            var buffer = Regulus.Serializer.TypeHelper.Serializer(flags);
            var flags2 = Regulus.Serializer.TypeHelper.Deserialize<Regulus.CustomType.Flag<TESTENUM>>(buffer);


            Assert.AreEqual(false, flags2[TESTENUM._2]);
            Assert.AreEqual(true, flags2[TESTENUM._3]);
        }

         [TestMethod]
        public void TestCustomFlagConvert()
        {
            var flags = new Regulus.CustomType.Flag<TESTENUM>(new[] { TESTENUM._1, TESTENUM._3 });
            object[] objs = (from e in flags select (object)e).ToArray();


            Regulus.CustomType.Flag<TESTENUM> flag2 = objs;

            Assert.AreEqual(false, flag2[TESTENUM._2]);
            Assert.AreEqual(true, flag2[TESTENUM._3]);
        }
    }
}
