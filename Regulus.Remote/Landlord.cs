using System.Collections.Generic;

namespace Regulus.Remote
{
    internal class LongProvider : ILandlordProviable<long>
    {
        
        
        long _Current;
        public LongProvider()
        {
            _Current = 0;
        }



        long ILandlordProviable<long>.Spawn()
        {
            return ++_Current;
        }
    }
    public class Landlord<T>
    {

        readonly System.Collections.Concurrent.ConcurrentQueue<T> _Enrollments;
        readonly ILandlordProviable<T> _Provider;
        public Landlord(ILandlordProviable<T> provider)
        {
            _Provider = provider;
            _Enrollments = new System.Collections.Concurrent.ConcurrentQueue<T>();
        }

        public T Rent()
        {
            T id;
            if(_Enrollments.TryDequeue(out id))
            {
                return id;
            }            

            return _Provider.Spawn();


        }
        public void Return(T obj)
        {
            _Enrollments.Enqueue(obj);
        }

    }



}
