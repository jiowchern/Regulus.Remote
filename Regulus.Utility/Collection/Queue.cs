using System;
using System.Collections.Generic;
using System.Threading;

namespace Regulus.Collection
{
	public class Queue<T>
	{
		private readonly System.Collections.Generic.Queue<T> _Set;
	    private SpinLock _Lock;
        public Queue()
		{
		    _Lock = new SpinLock();
            _Set = new System.Collections.Generic.Queue<T>();
		}

		public Queue(params T[] objs)
		{
		    _Lock = new SpinLock();
            _Set = new System.Collections.Generic.Queue<T>(objs);
		}

		public void Enqueue(T package)
		{
		    bool gotLock = false;
            try
		    {		        
		        _Lock.Enter(ref gotLock);
		        _Set.Enqueue(package);
            }
		    finally 
		    {
		        if(gotLock)
		            _Lock.Exit();

            }

            
		}

		public bool TryDequeue(out T obj)
		{
		    obj = default(T);
            bool result = false;
		    bool gotLock = false;
		    try
		    {
		        _Lock.Enter(ref gotLock);
		        if (_Set.Count > 0)
		        {
		            obj = _Set.Dequeue();
		            result = true;
		        }
            }
		    finally
		    {
		        if (gotLock)
		            _Lock.Exit();

		    }
          

			
			return result;
		}

		public T[] DequeueAll()
		{
		    bool gotLock = false;

		    T[] all;
		   
		    try
		    {
		        _Lock.Enter(ref gotLock);
		        all = _Set.ToArray();
		        _Set.Clear();
            }
		    finally 
		    {
		        if (gotLock)
		            _Lock.Exit();

            }
		    return all;


            
		}
	}
}
