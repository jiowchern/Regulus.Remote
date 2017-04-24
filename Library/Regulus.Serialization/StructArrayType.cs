using System;
using System.Runtime.InteropServices;

using Regulus.Serialization.Expansion;

namespace Regulus.Serialization
{
    public class StructArrayType<T> : ITypeDescriber where T : struct
    {
        private readonly int _Id;

        public StructArrayType(int id)
        {
            _Id = id;
        }

        int ITypeDescriber.Id
        {
            get { return _Id; }
        }

        Type ITypeDescriber.Type
        {
            get { return typeof (T[]); }
        }

        object ITypeDescriber.Default
        {
            get { return default(T[]); }
        }

        int ITypeDescriber.GetByteCount(object instance)
        {
            var array = instance as T[];

            var lenCount = Serializer.Varint.GetByteCount(array.Length);

            return lenCount + array.Length * Marshal.SizeOf(typeof(T));
        }

        int ITypeDescriber.ToBuffer(object instance, byte[] buffer, int begin)
        {
            var array = instance as T[];
            int offset = begin;
            offset += Serializer.Varint.NumberToBuffer(buffer, offset, array.Length);

            for (int i = 0; i < array.Length; i++)
            {
                var obj = array[i];

                int readed;
                obj.ToBytes(buffer, offset, out readed);
                offset += readed;
            }

            return offset - begin;
        }

        int ITypeDescriber.ToObject(byte[] buffer, int begin, out object instnace)
        {
            int offset = begin;
            ulong len;
            offset += Serializer.Varint.BufferToNumber(buffer, offset, out len);


            var array = new T[len];
            for (var i = 0ul; i < len; i++)
            {
                int readed;
                array[i] = buffer.ToStruct<T>(offset, out readed);

                offset += readed;
            }
            instnace = array;
            return offset - begin;
        }

        void ITypeDescriber.SetMap(ITypeDescriber[] describer)
        {
            
        }
    }
}