using System;
using System.Collections.Generic;
using System.IO;

namespace Regulus.Network.RUDP
{
    
    public interface IRecycleable<T> 
    {
        void Reset(T instance);
    }

    

    public sealed class ObjectPool<TObject,TShell> 
        where TShell : class ,IRecycleable<TObject> , new()
    {        
        private readonly IObjectProvider<TShell, TObject > _Provider;
        private readonly Queue<TObject> _Deads;

        class Resource
        {
            public readonly TObject Fillings;
            public readonly WeakReference Shell;
            

            public Resource(object shell, TObject get_fillings)
            {
                Shell = new WeakReference(shell);
                Fillings = get_fillings;
            }
        }

        
        private Queue<Resource> _Alives1;
        private Queue<Resource> _Alives2;

        public ObjectPool(IObjectFactory<TObject> factory) : this(new ObjectProvider<TShell, TObject>(factory))
        {
            
        }
        public ObjectPool(IObjectProvider<TShell, TObject> provider)
        {
            

            _Alives1 = new Queue<Resource>();
            _Alives2 = new Queue<Resource>();
            _Provider = provider;
            _Deads = new Queue<TObject>();
        }
        public TShell Spawn()
        {
            lock (this)
            {
                while (_Alives1.Count > 0)
                {
                    var resource = _Alives1.Dequeue();
                    if (resource.Shell.IsAlive == false)
                    {
                        _Deads.Enqueue(resource.Fillings);
                    }
                    else
                    {
                        _Alives2.Enqueue(resource);
                    }
                }

                var alive = _Alives1;
                _Alives1 = _Alives2;
                _Alives2 = alive;

                if (_Deads.Count > 0)
                {
                    var obj = _Deads.Dequeue();
                    var shell = _Provider.Reset(obj);
                    _Alives1.Enqueue(new Resource(shell, obj));
                    return shell;
                }
                else
                {
                    TShell shell;
                    TObject fillings;
                    _Provider.Supply(out shell, out fillings);
                    _Alives1.Enqueue(new Resource(shell, fillings));
                    return shell;
                }
            }
            

            


            
        }



    }


    
    public interface IObjectProvider<TShell , TObject>
    {
        void Supply(out TShell shell , out TObject fillings);
        TShell Reset(TObject fillings);
    }

    public class  ObjectProvider<TShell, TObject> : IObjectProvider<TShell, TObject> 
        where TShell : IRecycleable<TObject> , new () 

    {
        private readonly IObjectFactory<TObject> _Factory;

        public ObjectProvider(IObjectFactory<TObject> factory)
        {
            _Factory = factory;
        }

        void IObjectProvider<TShell, TObject>.Supply(out TShell shell, out TObject fillings)
        {
            shell = new TShell();
            fillings = _Factory.Spawn();
            shell.Reset(fillings);
        }

        TShell IObjectProvider<TShell, TObject>.Reset(TObject fillings)
        {
            var shell = new TShell();
            shell.Reset(fillings);
            return shell;
        }
    }

    public interface IObjectFactory<T>
    {
        T Spawn();
    }
}