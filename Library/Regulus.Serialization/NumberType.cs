using System;
using System.Diagnostics;

namespace Regulus.Serialization
{
    public class NumberType<T> : ITypeDescriber
    {
        private int _Id     ;

        public NumberType(int id)
        {
            _Id = id;
        }

        int ITypeDescriber.Id
        {
            get { return _Id; }
        }

        public Type Type { get { return typeof (T); } }

        int ITypeDescriber.GetByteCount(object instance)
        {

            
            
            

            return Serializer.Varint.GetByteCount(Convert.ToUInt64(instance));
        }

        int ITypeDescriber.ToBuffer(object instance, byte[] buffer, int begin)
        {

            return Serializer.Varint.NumberToBuffer(buffer, begin, Convert.ToUInt64(instance));

            
        }

        int ITypeDescriber.ToObject(byte[] buffer, int begin, out object instance)
        {

            instance = null;

            ulong value;            
            var readed = Serializer.Varint.BufferToNumber(buffer, begin, out value);

            if(typeof(int) == typeof(T))
                instance = (int)value;

            if (typeof(uint) == typeof(T))
                instance = (uint)value;

            if (typeof(long) == typeof(T))
                instance = (long)value;

            if (typeof(ulong) == typeof(T))
                instance = (ulong)value;
            

            return readed;
        }

        public void SetMap(ITypeDescriber[] describer)
        {
            
        }
    }
}