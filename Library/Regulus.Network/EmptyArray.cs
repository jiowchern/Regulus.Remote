namespace Regulus.Network.RUDP
{
    public class EmptyArray<T>
    {
        private readonly static T[] _Empty = new T[0];
        public static implicit operator T[] (EmptyArray<T> ea)
        {
            return _Empty;
        }
    }
}