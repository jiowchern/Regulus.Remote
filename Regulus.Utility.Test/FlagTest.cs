using Regulus.Utility;
using System.Linq;

namespace RegulusLibraryTest
{
    internal enum TESTENUM
    {
        _1,

        _2,

        _3
    };


    public class FlagTest
    {
        [NUnit.Framework.Test()]
        public void TestToArray()
        {
            Flag<TESTENUM> flags = new Flag<TESTENUM>();
            flags[TESTENUM._1] = true;
            flags[TESTENUM._2] = false;
            flags[TESTENUM._3] = true;

            TESTENUM[] array = flags.ToArray();

            NUnit.Framework.Assert.AreNotEqual(TESTENUM._2, array[0]);
            NUnit.Framework.Assert.AreNotEqual(TESTENUM._2, array[1]);
        }

        [NUnit.Framework.Test()]
        public void TestCustomFlag1()
        {
            Flag<TESTENUM> flags = new Flag<TESTENUM>();
            flags[TESTENUM._1] = false;
            flags[TESTENUM._2] = true;
            flags[TESTENUM._3] = false;

            NUnit.Framework.Assert.AreEqual(true, flags[TESTENUM._2]);
            NUnit.Framework.Assert.AreEqual(false, flags[TESTENUM._3]);
        }

        [NUnit.Framework.Test()]
        public void TestCustomFlag2()
        {
            Flag<TESTENUM> flags = new Flag<TESTENUM>(TESTENUM._1, TESTENUM._3);

            NUnit.Framework.Assert.AreEqual(false, flags[TESTENUM._2]);
            NUnit.Framework.Assert.AreEqual(true, flags[TESTENUM._3]);
        }


        // todo : 不能序列化 , 序列化陣列要改成IList檢查
        /*estMethod]
		public void TestCustomFlagSerializer()
		{
            
            var flags = new Flag<TESTENUM>(TESTENUM._1, TESTENUM._3);

			var buffer = TypeHelper.Serializer(flags);
			var flags2 = TypeHelper.Deserialize<Flag<TESTENUM>>(buffer);

			NUnit.Framework.Assert.AreEqual(false, flags2[TESTENUM._2]);
			NUnit.Framework.Assert.AreEqual(true, flags2[TESTENUM._3]);
		}*/

        [NUnit.Framework.Test()]
        public void TestCustomFlagConvert()
        {
            Flag<TESTENUM> flags = new Flag<TESTENUM>(TESTENUM._1, TESTENUM._3);
            object[] objs = (from e in flags select (object)e).ToArray();

            Flag<TESTENUM> flag2 = objs;

            NUnit.Framework.Assert.AreEqual(false, flag2[TESTENUM._2]);
            NUnit.Framework.Assert.AreEqual(true, flag2[TESTENUM._3]);
        }
    }
}
