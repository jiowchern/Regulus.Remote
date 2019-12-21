using System;
using System.Linq;

using System.Reflection;
using System.Runtime.Serialization;

namespace Regulus.Serialization
{
    public class ClassDescriber : ITypeDescriber 
    {
        

        private readonly Type _Type;

        private readonly FieldInfo[] _Fields;
                
        private readonly object _Default;

        private readonly IDescribersFinder _TypeSet;

        public ClassDescriber(Type type , IDescribersFinder finder)
        {
            _Default = null;            
            _Type = type;
            _TypeSet = finder;
            _Fields = (from field in _Type.GetFields()
                         where field.IsStatic == false && field.IsPublic orderby field.Name
                         select field).ToArray();
        }

        Type ITypeDescriber.Type
        {
            get { return _Type; }
        }
        
        public object Default { get { return _Default; } }

        int ITypeDescriber.GetByteCount(object instance)
        {
            try
            {
                var validFields = _Fields.Select(
                       (field, index) => new
                       {
                           field,
                           index
                       }).Where(validField => object.Equals(_GetDescriber(validField.field).Default, validField.field.GetValue(instance)) == false).ToArray();


                var validCount = Varint.GetByteCount(validFields.Length);
                int count = 0;
                for (int i = 0; i < validFields.Length; i++)
                {

                    var validField = validFields[i];
                    var value = validField.field.GetValue(instance);
                    var valueType = value.GetType();
                    var valueTypeCount = _TypeSet.Get().GetByteCount(valueType);
                    var describer = _TypeSet.Get(valueType);
                    var byteCount = describer.GetByteCount(value);

                    var indexCount = Varint.GetByteCount(validField.index);
                    count += byteCount + indexCount + valueTypeCount;
                }
                return count + validCount;
            }
            catch (Exception ex)
            {
                throw new DescriberException(typeof(ClassDescriber), _Type, "GetByteCount", ex);
            }
            
        }

        private ITypeDescriber _GetDescriber(FieldInfo field)
        {
            return _TypeSet.Get(field.FieldType);
        }

        int ITypeDescriber.ToBuffer(object instance, byte[] buffer, int begin)
        {

            try
            {
                int offset = begin;
                var validFields = _Fields.Select(
                           (field, index) => new
                           {
                               field,
                               index
                           }).Where(validField => object.Equals(_GetDescriber(validField.field).Default, validField.field.GetValue(instance)) == false)
                       .ToArray();

                offset += Varint.NumberToBuffer(buffer, offset, validFields.Length);


                foreach (var validField in validFields)
                {
                    var index = validField.index;
                    offset += Varint.NumberToBuffer(buffer, offset, index);
                    var field = validField.field;
                    var value = field.GetValue(instance);
                    var valueType = value.GetType();
                    offset += _TypeSet.Get().ToBuffer(valueType, buffer, offset);
                    var describer = _TypeSet.Get(valueType);
                    offset += describer.ToBuffer(value, buffer, offset);
                }


                return offset - begin;
            }
            catch (Exception ex)
            {

                throw new DescriberException(typeof(ClassDescriber), _Type, "ToBuffer", ex);
            }
            
        }

        int ITypeDescriber.ToObject(byte[] buffer, int begin , out object instance)
        {
            try
            {
                var constructor = _Type.GetConstructors().OrderBy(c => c.GetParameters().Length).Select(c => c).FirstOrDefault();
                if (constructor != null)
                {
                    var argTypes = constructor.GetParameters().Select(info => info.ParameterType).ToArray();
                    var objArgs = new object[argTypes.Length];

                    for (int i = 0; i < argTypes.Length; i++)
                    {
                        objArgs[i] = Activator.CreateInstance(argTypes[i]);
                    }
                    instance = Activator.CreateInstance(_Type, objArgs);
                }
                else
                {
                    instance = Activator.CreateInstance(_Type);
                }

                

                var offset = begin;

                ulong validLength;
                offset += Varint.BufferToNumber(buffer, offset, out validLength);

                for (var i = 0ul; i < validLength; i++)
                {
                    ulong index;
                    offset += Varint.BufferToNumber(buffer, offset, out index);
                    Type valueType;
                    offset += _TypeSet.Get().ToObject(buffer, offset, out valueType);
                    var filed = _Fields[index];
                    var describer = _TypeSet.Get(valueType);
                    object valueInstance;
                    offset += describer.ToObject(buffer, offset, out valueInstance);
                    filed.SetValue(instance, valueInstance);
                }

                return offset - begin;

            }
            catch (Exception ex)
            {

                throw new DescriberException(typeof(ClassDescriber), _Type, "ToObject", ex);
            }
            

        }

        
    }
}
