namespace RegulusLibraryTest
{
    public interface ICallTester
    {
        void Function1();

        void Function2(int a);

        int Function3();

        int Function4(int a, byte b, float c);

        int Function5 { get; }
    }

    public class CallTester
    {
        public void Function1()
        {
        }

        public void Function2(int a)
        {
        }
    }
}
