using System.Collections.Generic;

namespace Regulus.Network.RUDP
{
    internal class ArrayCache<T> 
    {
        private readonly int _Capacity;
        private readonly Queue<T[]> _Stock;
        public ArrayCache(int element_capacity)
        {
            _Stock = new Queue<T[]>();
            _Capacity = element_capacity;
        }

        public T[] Alloc()
        {
            if (_Stock.Count > 0)
            {
                return _Stock.Dequeue();
            }
            return new T[_Capacity];
        }

        public void Free(T[] buffer)
        {
            if(buffer.Length != _Capacity)
                throw new System.Exception("Buffer length does not match.");

            _Stock.Enqueue(buffer);
        }
    }
}