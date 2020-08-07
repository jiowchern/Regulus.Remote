using System.Collections.Generic;

namespace Regulus.Network
{
    internal class ArrayCache<T>
    {
        private readonly int m_Capacity;
        private readonly Queue<T[]> m_Stock;
        public ArrayCache(int ElementCapacity)
        {
            m_Stock = new Queue<T[]>();
            m_Capacity = ElementCapacity;
        }

        public T[] Alloc()
        {
            if (m_Stock.Count > 0)
                return m_Stock.Dequeue();
            return new T[m_Capacity];
        }

        public void Free(T[] Buffer)
        {
            if (Buffer.Length != m_Capacity)
                throw new System.Exception("Package length does not match.");

            m_Stock.Enqueue(Buffer);
        }
    }
}