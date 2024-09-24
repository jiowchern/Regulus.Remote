using System;

namespace Regulus.Serialization
{
    public class BufferDescriber<T> : BufferDescriber
    {
        public BufferDescriber() : base(typeof(T))
        {

        }
    }
    public class BufferDescriber : ITypeDescriber
    {
        private readonly Type _Type;


        public BufferDescriber(Type type)
        {
            if (type.IsArray == false)
                throw new ArgumentException("type is not array " + type.FullName);
            _Type = type;

        }



        Type ITypeDescriber.Type
        {
            get { return _Type; }
        }

        object ITypeDescriber.Default
        {
            get { return null; }
        }

        int ITypeDescriber.GetByteCount(object instance)
        {
            Array src = instance as Array;
            int bufferLength = Buffer.ByteLength(src);
            int bufferLen = Varint.GetByteCount(bufferLength);
            int elementLen = Varint.GetByteCount(src.Length);
            return bufferLen + bufferLength + elementLen;
        }

        int ITypeDescriber.ToBuffer(object instance, Regulus.Memorys.Buffer buffer, int begin)
        {
            var bytes = buffer.Bytes;
            Array src = instance as Array;
            int bufferLength = Buffer.ByteLength(src);
            int offset = begin;
            offset += Varint.NumberToBuffer(bytes.Array , bytes.Offset + offset, bufferLength);
            offset += Varint.NumberToBuffer(bytes.Array, bytes.Offset + offset, src.Length);



            Buffer.BlockCopy(src, 0, bytes.Array, bytes.Offset + offset, bufferLength);
            return offset - begin + bufferLength;
        }

        int ITypeDescriber.ToObject(Regulus.Memorys.Buffer buffer, int begin, out object instnace)
        {
            int offset = begin;
            int bufferLen = 0;
            offset += Varint.BufferToNumber(buffer, offset, out bufferLen);
            int elementLen = 0;
            offset += Varint.BufferToNumber(buffer, offset, out elementLen);
            Array dst = _Create(elementLen);

            var bytes = buffer.Bytes;
            Buffer.BlockCopy(bytes.Array, bytes.Offset + offset, dst, 0, bufferLen);

            instnace = dst;
            return offset - begin + bufferLen;
        }

        private Array _Create(int len)
        {
            return Activator.CreateInstance(_Type, new object[] { len }) as Array;
        }


    }
}