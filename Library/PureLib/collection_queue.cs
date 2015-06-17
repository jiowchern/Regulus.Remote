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

        public Queue(params T[] objs)
        {
            _Set = new System.Collections.Generic.Queue<T>(objs);
        
        }
        public void Enqueue(T package)
        {
            lock (_Set)
            {
                _Set.Enqueue(package);
            }

        }




        public bool TryDequeue(out T obj)
        {
            lock (_Set)
            {
                if (_Set.Count > 0)
                {
                    obj = _Set.Dequeue();
                    return true;
                }                    
            }
            obj = default(T);
            return false;
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
