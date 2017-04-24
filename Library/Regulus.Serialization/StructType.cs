using System;
using System.Runtime.InteropServices;

using Regulus.Serialization.Expansion;

namespace Regulus.Serialization
{
    public class StructType<T> : ITypeDescriber where T : struct
    {
        private readonly int _Id;

        public StructType(int id)
        {
            _Id = id;
        }

        int ITypeDescriber.Id
        {
            get { return _Id; }
        }

        Type ITypeDescriber.Type
        {
            get { return typeof (T); }
        }

        object ITypeDescriber.Default
        {
            get { return typeof (T); }
        }

        int ITypeDescriber.GetByteCount(object instance)
        {
            return Marshal.SizeOf(typeof(T));
        }

        int ITypeDescriber.ToBuffer(object instance, byte[] buffer, int begin)
        {
            T obj = (T)instance;

            var offset = begin;
            int readCount;
            obj.ToBytes(buffer, offset, out readCount);

            return readCount;
        }

        int ITypeDescriber.ToObject(byte[] buffer, int begin, out object instnace)
        {
            int readed;
            instnace = buffer.ToStruct<T>(begin, out readed);
            return readed;
        }

        void ITypeDescriber.SetMap(ITypeDescriber[] describer)
        {
            
        }
    }
}