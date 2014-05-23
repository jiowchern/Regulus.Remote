using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility
{

    public class Poller<T> where T : class
    {
        Queue<T> _Adds = new Queue<T>();
        Queue<Func<T, bool>> _Removes = new Queue<Func<T, bool>>();

        List<T> _Objects = new List<T>();
        
        public void Add(T obj)
        {            
            lock (_Adds)
                _Adds.Enqueue(obj);
        }


        public void Remove(Func<T, bool> obj)
        {
            lock (_Removes)
                _Removes.Enqueue(obj);
        }

        public T[] UpdateSet()
        {
            lock (_Objects)
            {
                lock (_Adds)
                    _Add(_Adds);
                lock (_Removes)
                    _Remove(_Removes);

                return _Objects.ToArray();
            }            
        }

        private void _Remove(Queue<Func<T, bool>> removes)
        {
            while (removes.Count > 0)
            {
                var obj = removes.Dequeue();

                _Objects.RemoveAll(o => obj.Invoke(o) );
            }
        }

        private void _Add(Queue<T> adds)
        {
            while (adds.Count > 0)
            {
                var obj = adds.Dequeue();
                _Objects.Add(obj);
            }
        }
    }
}
