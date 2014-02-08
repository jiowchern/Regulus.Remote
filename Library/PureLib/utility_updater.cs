using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility
{
    public interface IUpdatable : Regulus.Framework.ILaunched
    {
        bool Update();
    }



    public class Updater<T> where T : IUpdatable
    {
        Queue<T> _Adds = new Queue<T>();
        Queue<T> _Removes = new Queue<T>();

        List<T> _Ts = new List<T>();

        public T[] Objects { get { return _Ts.ToArray(); } }

        public void Add(T framework)
        {
            if (framework != null)
                _Adds.Enqueue(framework);
        }

        public void Remove(T framework)
        {
            if (framework != null)
                _Removes.Enqueue(framework);
        }

        public void Update()
        {
            _Remove(_Removes, _Ts);

            _Add(_Adds, _Ts);

            _Update();
        }

        private void _Remove(Queue<T> remove_framework, List<T> frameworks)
        {
            while (remove_framework.Count > 0)
            {
                var fw = remove_framework.Dequeue();
                frameworks.Remove(fw);
                fw.Shutdown();
            }
        }

        private void _Add(Queue<T> add_frameworks, List<T> frameworks)
        {
            while (add_frameworks.Count > 0)
            {
                var fw = add_frameworks.Dequeue();
                frameworks.Add(fw);
                fw.Launch();
            }
        }

        private void _Update()
        {
            foreach (var framework in _Ts)
            {
                if (framework.Update() == false)
                {
                    Remove(framework);
                }
            }

        }

        private void _Shutdown(List<T> frameworks)
        {

            foreach (var framework in frameworks)
            {
                framework.Shutdown();
            }
        }

        public void Shutdown()
        {
            _Shutdown(_Ts);
            _Ts.Clear();
        }
    }

    public class Updater : Updater<IUpdatable>
    { 

    }
}
