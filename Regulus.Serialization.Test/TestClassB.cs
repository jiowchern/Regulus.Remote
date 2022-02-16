namespace Regulus.Serialization.Tests
{
    public class TestClassB
    {
        public int Data;
    }


    public class TestParent
    {
        public int Data;
    }

    public class TestChild : TestParent
    {
        public int Data;
    }

    public class TestGrandson : TestChild
    {
        public int Data;
    }

    public class TestPoly
    {
        public TestParent Parent;
    }

    public class TestClassCycle1
    {

        public TestClassCycle2 Field1;
        public int Field2;
    }

    public class TestClassCycle2
    {
        public TestClassCycle1 Field1;
        public int Field2;
    }


}