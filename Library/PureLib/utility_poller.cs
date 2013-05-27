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

        public List<T> Objects { get { return _Objects; } }
        public void Add(T obj)
        {
            _Adds.Enqueue(obj);
        }


        public void Remove(Func<T, bool> obj)
        {
            _Removes.Enqueue(obj);
        }

        public List<T> Update()
        {
            _Add(_Adds);
            _Remove(_Removes);

            return Objects;
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
