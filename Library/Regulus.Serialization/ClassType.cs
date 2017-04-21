using System;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace Regulus.Serialization
{
    public class ClassType<T> : ITypeDescriber
    {
        private readonly int _Id;
        private FieldInfo[] _Fields;
        private ITypeDescriber[] _Describers;

        public ClassType(int id)
        {
            _Id = id;

            _Fields = (from field in typeof(T).GetFields()
                         where field.IsStatic == false && field.IsPublic orderby field.Name
                         select field).ToArray();
        }

        int ITypeDescriber.Id
        {
            get { return _Id; }
        }

        Type ITypeDescriber.Type
        {
            get { return typeof(T); }
        }

        int ITypeDescriber.GetByteCount(object instance)
        {
            int count = 0;
            for (int i = 0; i < _Fields.Length; i++)
            {
                
                var field = _Fields[i];
                var value = field.GetValue(instance);
                var describer = _GetDescriber(field);
                var byteCount = describer.GetByteCount(value);

                var indexCount = Serializer.Varint.GetByteCount(1UL + (ulong)i);
                count += byteCount + indexCount;
            }
            return count;
        }

        private ITypeDescriber _GetDescriber(FieldInfo field)
        {
            return _Describers.First(d => d.Type == field.FieldType);
        }

        int ITypeDescriber.ToBuffer(object instance, byte[] buffer, int begin)
        {

            int offset = begin;
            for (int i = 0; i < _Fields.Length; i++)
            {
                offset += Serializer.Varint.NumberToBuffer(buffer, offset, i + 1);
                var field = _Fields[i];
                var value = field.GetValue(instance);
                var describer = _GetDescriber(field);
                offset += describer.ToBuffer(value, buffer, offset);
            }

            return offset - begin;
        }

        int ITypeDescriber.ToObject(byte[] buffer, int begin , out object instance)
        {
            instance = Activator.CreateInstance(typeof (T));

            var offset = begin;

            while (offset < buffer.Length)
            {
                ulong index;
                offset += Serializer.Varint.BufferToNumber(buffer, offset, out index);
                index--;
                var filed = _Fields[index];
                var describer = _GetDescriber(filed);
                object valueInstance;
                offset += describer.ToObject(buffer, offset, out valueInstance);
                filed.SetValue(instance , valueInstance);
            }
            return offset;

        }

        void ITypeDescriber.SetMap(ITypeDescriber[] describers)
        {
            _Describers = describers;
        }
    }
}