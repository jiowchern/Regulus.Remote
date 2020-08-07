using System.Collections.Generic;

namespace Regulus.Remote
{
    internal class LongProvider : ILandlordProviable<long>
    {
        internal static readonly long OnlineId = 1;
        internal static readonly long ConnectId = 2;
        long _Current;
        public LongProvider()
        {
            _Current = 3;
        }



        long ILandlordProviable<long>.Spawn()
        {
            return _Current++;
        }
    }
    public class Landlord<T>
    {

        readonly Queue<T> _Enrollments;
        readonly ILandlordProviable<T> _Provider;
        public Landlord(ILandlordProviable<T> provider)
        {
            _Provider = provider;
            _Enrollments = new Queue<T>();
        }

        public T Rent()
        {

            if (_Enrollments.Count > 0)
                return _Enrollments.Dequeue();

            return _Provider.Spawn();


        }
        public void Return(T obj)
        {
            _Enrollments.Enqueue(obj);
        }

    }



}
