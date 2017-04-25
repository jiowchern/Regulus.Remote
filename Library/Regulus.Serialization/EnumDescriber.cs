using System;
using System.CodeDom;

namespace Regulus.Serialization
{

    public class EnumDescriber<T> : EnumDescriber
    {
        public EnumDescriber(int id) : base(id, typeof (T))
        {
            
        }
    }
    public class EnumDescriber : ITypeDescriber 
    {
        private readonly int _Id;

        private readonly Type _Type;

        private readonly object _Default;

        public EnumDescriber(int id , Type type)
        {
            _Id = id;
            _Type = type;

            _Default = Activator.CreateInstance(type);
        }

        int ITypeDescriber.Id
        {
            get { return _Id; }
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
            return Serializer.Varint.GetByteCount(Convert.ToUInt64((int)instance));
        }

        int ITypeDescriber.ToBuffer(object instance, byte[] buffer, int begin)
        {
            return Serializer.Varint.NumberToBuffer(buffer, begin, Convert.ToUInt64((int)instance));
        }

        int ITypeDescriber.ToObject(byte[] buffer, int begin, out object instnace)
        {
            ulong value;
            var readed = Serializer.Varint.BufferToNumber(buffer, begin, out value);            
            var values = Enum.GetValues(_Type);
            instnace = values.GetValue((int)value);
            return readed;
        }

        void ITypeDescriber.SetMap(ITypeDescriber[] describer)
        {
            
        }
    }
}