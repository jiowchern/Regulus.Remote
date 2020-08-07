using System;

namespace Regulus.Serialization
{

    public class EnumDescriber<T> : EnumDescriber
    {
        public EnumDescriber() : base(typeof(T))
        {

        }
    }
    public class EnumDescriber : ITypeDescriber
    {


        private readonly Type _Type;

        private readonly object _Default;

        public EnumDescriber(Type type)
        {

            _Type = type;

            _Default = Activator.CreateInstance(type);
        }



        Type ITypeDescriber.Type
        {
            get { return _Type; }
        }

        object ITypeDescriber.Default
        {
            get { return _Default; }
        }

        int ITypeDescriber.GetByteCount(object instance)
        {
            return Varint.GetByteCount(Convert.ToUInt64(instance));
        }

        int ITypeDescriber.ToBuffer(object instance, byte[] buffer, int begin)
        {
            return Varint.NumberToBuffer(buffer, begin, Convert.ToUInt64(instance));
        }

        int ITypeDescriber.ToObject(byte[] buffer, int begin, out object instnace)
        {
            ulong value;
            int readed = Varint.BufferToNumber(buffer, begin, out value);

            instnace = Enum.ToObject(_Type, value);
            return readed;
        }


    }
}