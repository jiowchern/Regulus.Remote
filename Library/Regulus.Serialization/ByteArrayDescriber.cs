using System;

namespace Regulus.Serialization
{
    public class ByteArrayDescriber : ITypeDescriber
    {
        private readonly int _Id;        

        public ByteArrayDescriber(int id)
        {
            _Id = id;
            
        }

        int ITypeDescriber.Id
        {
            get { return _Id; }
        }

        Type ITypeDescriber.Type
        {
            get { return typeof(byte[]); }
        }

        object ITypeDescriber.Default
        {
            get { return null; }
        }

        int ITypeDescriber.GetByteCount(object instance)
        {
            var array = instance as Array;
            var len = array.Length;
            var lenByetCount = Varint.GetByteCount(len);
            return len + lenByetCount;
        }

        int ITypeDescriber.ToBuffer(object instance, byte[] buffer, int begin)
        {
            var array = instance as byte[];
            var len = array.Length;

            var offset = begin;
            offset += Varint.NumberToBuffer(buffer, offset, len);
            for (int i = 0; i < len; i++)
            {
                buffer[offset++] = array[i];
            }
            return offset - begin;
        }

        int ITypeDescriber.ToObject(byte[] buffer, int begin, out object instnace)
        {
            int offset = begin;
            int len = 0;
            offset += Varint.BufferToNumber(buffer, offset, out len);
            var array = new byte[len];
            for (int i = 0; i < len; i++)
            {
                array[i] = buffer[offset++];
            }
            instnace = array;
            return offset - begin; 
        }

        void ITypeDescriber.SetMap(TypeSet type_set)
        {
            
        }
    }
}