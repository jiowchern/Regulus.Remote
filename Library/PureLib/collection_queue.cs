using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Collection
{
    public class Queue<T>
    {

        System.Collections.Generic.Queue<T> _Set;

        public Queue()
        {
            _Set = new System.Collections.Generic.Queue<T>();
        }
        public void Enqueue(T package)
        {
            lock (_Set)
            {
                _Set.Enqueue(package);
            }

        }

        public int Count
        {
            get
            {
                lock (_Set)
                    return _Set.Count;
            }
        }



        public T Dequeue()
        {
            lock (_Set)
            {

                return _Set.Dequeue();
            }
        }

        public T[] DequeueAll()
        {
            lock (_Set)
            {
                var all = _Set.ToArray();
                _Set.Clear();
                return all;
            }
        }
    }
}
