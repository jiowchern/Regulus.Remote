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

    public interface IUpdatable<T> : Regulus.Framework.ILaunched
    {
        bool Update(T arg);
    }

    public class Launcher<T > where T : Regulus.Framework.ILaunched
    {
        Queue<T> _Adds = new Queue<T>();
        Queue<T> _Removes = new Queue<T>();

        List<T> _Ts = new List<T>();

        public int Count { get { return _Ts.Count; } }

        public T[] Objects
        {
            get
            {                
                return _Ts.ToArray();                
            }
        }

        protected System.Collections.Generic.IEnumerable<T> _GetObjectSet()
        {
            lock (_Ts)
            {
                lock (_Removes)
                    _Remove(_Removes, _Ts);

                lock (_Adds)
                    _Add(_Adds, _Ts);

                return Objects;
                //return _Update();
            }
        }

        public void Add(T framework)
        {
            lock (_Adds) 
                _Adds.Enqueue(framework);            
        }

        public void Remove(T framework)
        {
            lock (_Removes) 
                _Removes.Enqueue(framework);            
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

        public void Shutdown()
        {
            _Shutdown(_Ts);
            _Ts.Clear();
        }

        private void _Shutdown(List<T> frameworks)
        {

            foreach (var framework in frameworks)
            {
                framework.Shutdown();
            }
        }
    }

    public class CenterOfUpdateableToGenerics<T> : Launcher<Regulus.Utility.IUpdatable<T>>             
    {
        public void Working(T arg)
        {
            foreach (var t in base._GetObjectSet())
            {
                if (t.Update(arg) == false)
                {
                    Remove(t);
                }                
            }
        }
    }

    public class CenterOfUpdateable : Launcher<Regulus.Utility.IUpdatable>
    {
        public void Working()
        {
            foreach (var t in base._GetObjectSet())
            {
                if (t.Update() == false)
                {
                    Remove(t);
                }
            }
        }
    }
}
