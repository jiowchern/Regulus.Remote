using System;

namespace Regulus.Serialization
{
    public class EnumType<T> : ITypeDescriber 
    {
        private readonly int _Id;

        public EnumType(int id)
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
            get { return default(T); }
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
            instnace = (T)Convert.ChangeType(value, typeof(int));
            return readed;
        }

        void ITypeDescriber.SetMap(ITypeDescriber[] describer)
        {
            
        }
    }
}