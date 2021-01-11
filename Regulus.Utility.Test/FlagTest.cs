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
        [Xunit.Fact]
        public void TestToArray()
        {
            Flag<TESTENUM> flags = new Flag<TESTENUM>();
            flags[TESTENUM._1] = true;
            flags[TESTENUM._2] = false;
            flags[TESTENUM._3] = true;

            TESTENUM[] array = flags.ToArray();

            Xunit.Assert.NotEqual(TESTENUM._2, array[0]);
            Xunit.Assert.NotEqual(TESTENUM._2, array[1]);
        }

        [Xunit.Fact]
        public void TestCustomFlag1()
        {
            Flag<TESTENUM> flags = new Flag<TESTENUM>();
            flags[TESTENUM._1] = false;
            flags[TESTENUM._2] = true;
            flags[TESTENUM._3] = false;

            Xunit.Assert.Equal(true, flags[TESTENUM._2]);
            Xunit.Assert.Equal(false, flags[TESTENUM._3]);
        }

        [Xunit.Fact]
        public void TestCustomFlag2()
        {
            Flag<TESTENUM> flags = new Flag<TESTENUM>(TESTENUM._1, TESTENUM._3);

            Xunit.Assert.Equal(false, flags[TESTENUM._2]);
            Xunit.Assert.Equal(true, flags[TESTENUM._3]);
        }


        // todo : 不能序列化 , 序列化陣列要改成IList檢查
        /*estMethod]
		public void TestCustomFlagSerializer()
		{
            
            var flags = new Flag<TESTENUM>(TESTENUM._1, TESTENUM._3);

			var buffer = TypeHelper.Serializer(flags);
			var flags2 = TypeHelper.Deserialize<Flag<TESTENUM>>(buffer);

			Xunit.Assert.Equal(false, flags2[TESTENUM._2]);
			Xunit.Assert.Equal(true, flags2[TESTENUM._3]);
		}*/

        [Xunit.Fact]
        public void TestCustomFlagConvert()
        {
            Flag<TESTENUM> flags = new Flag<TESTENUM>(TESTENUM._1, TESTENUM._3);
            object[] objs = (from e in flags select (object)e).ToArray();

            Flag<TESTENUM> flag2 = objs;

            Xunit.Assert.Equal(false, flag2[TESTENUM._2]);
            Xunit.Assert.Equal(true, flag2[TESTENUM._3]);
        }
    }
}
