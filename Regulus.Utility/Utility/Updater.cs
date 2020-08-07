using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Regulus.Utility
{
    /// <summary>
    ///     更新器
    /// </summary>
    public interface IUpdatable : IBootable
    {
        bool Update();
    }

    public interface IUpdatable<T> : IBootable
    {
        bool Update(T arg);
    }

    public class Launcher<T> where T : IBootable
    {
        enum TYPE
        {
            ADD, REMOVE
        }
        struct Operation
        {
            public TYPE Type;
            public T Item;
        }

        public event Action<T> AddEvent;
        public event Action<T> RemoveEvent;


        private readonly System.Collections.Concurrent.ConcurrentQueue<Operation> _Operations;

        private readonly System.Collections.Generic.List<T> _Items;


        public Launcher()
        {
            _Operations = new ConcurrentQueue<Operation>();

            _Items = new List<T>();
        }
        public int Count
        {
            get { return _Items.Count; }
        }

        protected T[] _Objects
        {

            get
            {
                T[] items;
                lock (_Items)
                {
                    items = _Items.ToArray();
                }

                return items;


            }
        }

        protected IEnumerable<T> _GetObjectSet()
        {
            Operation operation;
            while (_Operations.TryDequeue(out operation))
            {

                if (operation.Type == TYPE.ADD)
                {
                    lock (_Items)
                    {
                        _Items.Add(operation.Item);
                    }
                    operation.Item.Launch();

                    if (AddEvent != null)
                        AddEvent(operation.Item);
                }
                else
                {
                    lock (_Items)
                        _Items.Remove(operation.Item);
                    operation.Item.Shutdown();
                    if (RemoveEvent != null)
                        RemoveEvent(operation.Item);
                }
            }

            return _Objects;
        }

        public void Add(T item)
        {
            _Operations.Enqueue(new Operation() { Type = TYPE.ADD, Item = item });


        }

        public void Remove(T item)
        {
            _Operations.Enqueue(new Operation() { Type = TYPE.REMOVE, Item = item });
        }



        public void Shutdown()
        {
            lock (_Items)
            {
                foreach (T t in _Items)
                {
                    t.Shutdown();
                }
                _Items.Clear();
            }

            Operation operation;
            while (_Operations.TryDequeue(out operation))
            {

            }


        }


    }



    public class Updater<T> : Launcher<IUpdatable<T>>
    {
        public void Working(T arg)
        {
            foreach (IUpdatable<T> t in _GetObjectSet())
            {
                if (t.Update(arg) == false)
                {
                    Remove(t);
                }
            }
        }
    }

    public class Updater : Launcher<IUpdatable>
    {
        public void Working()
        {
            foreach (IUpdatable t in _GetObjectSet())
            {
                if (t.Update() == false)
                {
                    Remove(t);
                }
            }
        }
    }


}
