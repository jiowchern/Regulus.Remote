using System;
using System.Linq;

using System.Reflection;
using System.Runtime.Serialization;

namespace Regulus.Serialization
{
    public class ClassType : ITypeDescriber 
    {
        private readonly int _Id;

        private readonly Type _Type;

        private readonly FieldInfo[] _Fields;
        private ITypeDescriber[] _Describers;

        private object _Default;

        public ClassType(int id , Type type)
        {
            _Default = null;
            _Id = id;
            _Type = type;

            _Fields = (from field in _Type.GetFields()
                         where field.IsStatic == false && field.IsPublic orderby field.Name
                         select field).ToArray();
        }

        int ITypeDescriber.Id
        {
            get { return _Id; }
        }

        Type ITypeDescriber.Type
        {
            get { return _Type; }
        }
        
        public object Default { get { return _Default; } }

        int ITypeDescriber.GetByteCount(object instance)
        {

            var validFields = _Fields.Select(
                       (field, index) => new
                       {
                           field,
                           index
                       }).Where(validField => object.Equals(_GetDescriber(validField.field).Default, validField.field.GetValue(instance)) == false) .ToArray();


            var validCount = Serializer.Varint.GetByteCount(validFields.Length);
            int count = 0;
            for (int i = 0; i < validFields.Length; i++)
            {
                
                var validField = validFields[i];
                var value = validField.field.GetValue(instance);
                var describer = _GetDescriber(validField.field);
                var byteCount = describer.GetByteCount(value);

                var indexCount = Serializer.Varint.GetByteCount(validField.index);
                count += byteCount + indexCount;
            }
            return count + validCount;
        }

        private ITypeDescriber _GetDescriber(FieldInfo field)
        {
            return _Describers.First(d => d.Type == field.FieldType);
        }

        int ITypeDescriber.ToBuffer(object instance, byte[] buffer, int begin)
        {
            int offset = begin;
            var validFields = _Fields.Select(
                       (field, index) => new
                       {
                           field,
                           index
                       }).Where(validField => object.Equals(_GetDescriber(validField.field).Default, validField.field.GetValue(instance))== false)
                   .ToArray();

            offset += Serializer.Varint.NumberToBuffer(buffer, offset, validFields.Length);


            foreach (var validField in validFields)
            {
                var index = validField.index;
                offset += Serializer.Varint.NumberToBuffer(buffer, offset, index);
                var field = validField.field;
                var value = field.GetValue(instance);
                var describer = _GetDescriber(field);
                offset += describer.ToBuffer(value, buffer, offset);
            }
            

            return offset - begin;
        }

        int ITypeDescriber.ToObject(byte[] buffer, int begin , out object instance)
        {
            instance = Activator.CreateInstance(_Type);

            var offset = begin;

            ulong validLength;
            offset += Serializer.Varint.BufferToNumber(buffer, offset, out validLength);

            for (var i = 0ul; i < validLength; i++)
            {
                ulong index;
                offset += Serializer.Varint.BufferToNumber(buffer, offset, out index);

                var filed = _Fields[index];
                var describer = _GetDescriber(filed);
                object valueInstance;
                offset += describer.ToObject(buffer, offset, out valueInstance);
                filed.SetValue(instance, valueInstance);
            }

            return offset - begin;

        }

        void ITypeDescriber.SetMap(ITypeDescriber[] describers)
        {
            _Describers = describers;
        }
    }
}