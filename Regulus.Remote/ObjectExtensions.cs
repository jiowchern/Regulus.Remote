namespace Regulus.Remote.Extensions
{
    static class ObjectExtensions
    {
        public static bool IsNotType<T>(this object o1, out T o2) where T : class
        {
            o2 = default(T);
            if (o1 is T oo)
            {
                o2 = oo;
                return false;
            }
            return true;
        }
    }
}
