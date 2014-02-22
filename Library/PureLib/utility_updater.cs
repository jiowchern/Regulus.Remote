﻿using System;
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


    public class Launcher<T> where T : Regulus.Framework.ILaunched
    {
        
        Queue<T> _Adds = new Queue<T>();
        Queue<T> _Removes = new Queue<T>();

        List<T> _Ts = new List<T>();


        public T[] Objects
        {
            get
            {
                lock (_Ts)
                {
                    return _Ts.ToArray();
                }
            }
        }
        public int Count { get { return _Ts.Count; } }

        public void Add(T framework)
        {
            lock (_Ts)
            {
                if (framework != null)
                    _Adds.Enqueue(framework);
            }
        }

        public void Remove(T framework)
        {
            lock (_Ts)
            {
                if (framework != null)
                    _Removes.Enqueue(framework);
            }
        }

        public System.Collections.Generic.IEnumerable<T> Update()
        {
            lock (_Ts)
            {
                _Remove(_Removes, _Ts);

                _Add(_Adds, _Ts);

                return _Update();
            }
        }

        System.Collections.Generic.IEnumerable<T> _Update()
        {            
            return _Ts;
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

    public class Updater<T> : Launcher<Regulus.Utility.IUpdatable<T>>             
    {
        public void Update(T arg)
        {
            foreach (var t in base.Update())
            {
                if (t.Update(arg) == false)
                {
                    Remove(t);
                }                
            }
        }
    }


    public class Updater : Launcher<Regulus.Utility.IUpdatable>             
    {
        public void Update()
        {
            foreach (var t in base.Update())
            {
                if (t.Update() == false)
                {
                    Remove(t);
                }
            }
        }
    }
    
    
}
