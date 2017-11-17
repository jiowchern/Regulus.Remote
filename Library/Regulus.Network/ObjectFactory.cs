using System;
using System.Collections.Generic;

namespace Regulus.Network
{
    public sealed class ObjectFactory<TObject,TShell> 
        where TShell : class ,IRecycleable<TObject> , new()
    {
        private class Resource
        {
            public readonly TObject Fillings;
            public readonly WeakReference Shell;


            public Resource(TShell shell, TObject fillings)
            {
                Shell = new WeakReference(shell);
                Fillings = fillings;
            }
        }

        private readonly IObjectPool<TShell, TObject > m_Pool;
        private readonly Queue<TObject> m_Deads;
        private Queue<Resource> m_Alives1;
        private Queue<Resource> m_Alives2;

        public ObjectFactory(IObjectProvider<TObject> provider) : this(new ObjectPool<TShell, TObject>(provider))
        {
            
        }
        public ObjectFactory(IObjectPool<TShell, TObject> pool)
        {
            

            m_Alives1 = new Queue<Resource>();
            m_Alives2 = new Queue<Resource>();
            m_Pool = pool;
            m_Deads = new Queue<TObject>();
        }
        public TShell Spawn()
        {
            lock (this)
            {
                while (m_Alives1.Count > 0)
                {
                    var resource = m_Alives1.Dequeue();
                    if (resource.Shell.IsAlive == false)
                        m_Deads.Enqueue(resource.Fillings);
                    else
                        m_Alives2.Enqueue(resource);
                }

                var alive = m_Alives1;
                m_Alives1 = m_Alives2;
                m_Alives2 = alive;

                if (m_Deads.Count > 0)
                {
                    var fillings = m_Deads.Dequeue();
                    TShell shell;
                    m_Pool.Reset(out shell , fillings);
                    m_Alives1.Enqueue(new Resource(shell, fillings));
                    return shell;
                }
                else
                {
                    TShell shell;
                    TObject fillings;
                    m_Pool.New(out shell, out fillings);
                    m_Alives1.Enqueue(new Resource(shell, fillings));
                    return shell;
                }

                
            }
            

            


            
        }



    }
}