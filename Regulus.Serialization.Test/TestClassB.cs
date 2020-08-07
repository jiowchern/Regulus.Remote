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
}