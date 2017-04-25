using System;
using System.Diagnostics;

namespace Regulus.Serialization
{


    public class NumberDescriber<T> : NumberDescriber
    {
        public NumberDescriber(int id) : base(id , typeof(T)) { }
    }
    public class NumberDescriber : ITypeDescriber 
    {
        private int _Id     ;

        private readonly Type _Type;

        private object _Default;

        public NumberDescriber(int id , Type type)
        {

            _Default = Activator.CreateInstance(type);
            _Id = id;
            _Type = type;
        }

        int ITypeDescriber.Id
        {
            get { return _Id; }
        }

        public Type Type { get { return _Type; } }

        int ITypeDescriber.GetByteCount(object instance)
        {
            return Serializer.Varint.GetByteCount(Convert.ToUInt64(instance));
        }
        public object Default { get { return _Default; } }
        int ITypeDescriber.ToBuffer(object instance, byte[] buffer, int begin)
        {            
            return Serializer.Varint.NumberToBuffer(buffer, begin, Convert.ToUInt64(instance));
        }

        int ITypeDescriber.ToObject(byte[] buffer, int begin, out object instance)
        {
            ulong value;            
            var readed = Serializer.Varint.BufferToNumber(buffer, begin, out value);
            instance = Convert.ChangeType(value , _Type);
            return readed;
        }

        public void SetMap(ITypeDescriber[] describer)
        {
            
        }
    }
}