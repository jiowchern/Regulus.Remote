using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Regulus.Serialization
{
    public class ClassArrayType<T> : ITypeDescriber
    {
        private int _Id;
        private ITypeDescriber[] _Describers;

        public ClassArrayType(int id)
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

        int ITypeDescriber.GetByteCount(object instance)
        {
            throw new NotImplementedException();
        }

        int ITypeDescriber.ToBuffer(object instance, byte[] buffer, int begin)
        {
            var array = instance as T[];
            var len = array.Length;

            var offset = begin;
            offset += Serializer.Varint.NumberToBuffer(buffer, offset, len);
            
            for (int i = 0; i < len; i++)
            {
                var obj = array[i];
                if (object.Equals(obj, default(T)))
                    continue;

                var describer = _GetDescriber(typeof (T));
                offset += describer.ToBuffer(obj, buffer, offset);
            }
        }

        int ITypeDescriber.ToObject(byte[] buffer, int begin, out object instnace)
        {
            throw new NotImplementedException();
        }

        void ITypeDescriber.SetMap(ITypeDescriber[] describers)
        {
            _Describers = describers;
        }

        private ITypeDescriber _GetDescriber(Type type)
        {
            return _Describers.First(d => d.Type == type);
        }
    }
}