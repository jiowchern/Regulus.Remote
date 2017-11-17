namespace Regulus.Network
{
    public class EmptyArray<T>
    {
        private static readonly T[] Empty = new T[0];
        public static implicit operator T[] (EmptyArray<T> Ea)
        {
            return Empty;
        }
    }
}