using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace Regulus.Serialization
{
    public class ArrayDescriber : ITypeDescriber 
    {        

        private readonly Type _Type;


        private readonly object _Default;
        private readonly object _DefaultElement;
        private readonly IDescribersFinder _TypeSet;

        public ArrayDescriber(Type type, IDescribersFinder finder)
        {

            _TypeSet = finder;
            _Default = null;            
            _Type = type;
            var elementType = type.GetElementType();
            try
            {
                if (!elementType.IsClass)
                    _DefaultElement = Activator.CreateInstance(elementType);
                else
                {
                    _DefaultElement = null;
                }
                
            }
            catch (Exception ex)
            {
                
                throw new DescriberException(typeof(ArrayDescriber), _Type, "_DefaultElement", ex);
            }
            
        }

        

        Type ITypeDescriber.Type
        {
            get { return _Type; }
        }

        public object Default { get { return _Default; } }


        struct ValidObject
        {
            public int Index;
            public object Object;
        }


        struct ValidObjectSet
        {
            public int TotalLength;
            public int ValidLength;

            public ValidObject[] ValidObjects;
        }

        private ValidObjectSet _GetSet(object instance)
        {
            var array = instance as IList;

            int validLength = 0;
            for (int i = 0; i < array.Count; i++)
            {
                var obj = array[i];                
                if (object.Equals(obj, _DefaultElement) == false)
                {
                    validLength++;
                }
            }
            

            
            var validObjects = new List<ValidObject>();
            for (int i = 0; i < array.Count; i++)
            {
                var obj = array[i];
                var index = i;
                if (object.Equals(obj, _DefaultElement) == false)
                {

                    validObjects.Add( new ValidObject() { Index = index , Object =  obj});                    
                }
            }

            return new ValidObjectSet()
            {
                TotalLength = array.Count,
                ValidLength = validLength,
                ValidObjects = validObjects.ToArray()
            };
        }
        int ITypeDescriber.GetByteCount(object instance)
        {
            var set = _GetSet(instance);


            var lenCount = Varint.GetByteCount(set.TotalLength);
            var validCount = Varint.GetByteCount(set.ValidLength);

            
            var instanceCount = 0;
            for (int i = 0; i < set.ValidObjects.Length; i++)
            {
                var index = set.ValidObjects[i].Index;
                var obj = set.ValidObjects[i].Object;
                
                var describer = _TypeSet.Get(obj.GetType());
                
                instanceCount += Varint.GetByteCount(index);
                instanceCount += _TypeSet.Get().GetByteCount(obj.GetType());
                instanceCount += describer.GetByteCount(obj);
            }

            return instanceCount + lenCount + validCount;
        }

        int ITypeDescriber.ToBuffer(object instance, byte[] buffer, int begin)
        {

            try
            {
                var set = _GetSet(instance);
                var offset = begin;
                offset += Varint.NumberToBuffer(buffer, offset, set.TotalLength);
                offset += Varint.NumberToBuffer(buffer, offset, set.ValidLength);

                
                for (int i = 0; i < set.ValidObjects.Length; i++)
                {
                    var index = set.ValidObjects[i].Index;
                    var obj = set.ValidObjects[i].Object;
                    offset += Varint.NumberToBuffer(buffer, offset, index);
                    var objType = obj.GetType();
                    var describer = _TypeSet.Get(objType);
                    offset += _TypeSet.Get().ToBuffer(objType, buffer, offset);
                    offset += describer.ToBuffer(obj, buffer, offset);
                }

                return offset - begin;
            }
            catch (Exception ex)
            {
                
                throw new DescriberException(typeof(ArrayDescriber) , _Type , "ToBuffer" ,ex);
            }
            
        }

        int ITypeDescriber.ToObject(byte[] buffer, int begin, out object instnace)
        {
            try
            {
                var offset = begin;
                ulong count;
                offset += Varint.BufferToNumber(buffer, offset, out count);
                var array = Activator.CreateInstance(_Type, (int)count) as IList;
                instnace = array;

                ulong validCount;
                offset += Varint.BufferToNumber(buffer, offset, out validCount);

                
                for (var i = 0UL; i < validCount; i++)
                {
                    var index = 0LU;

                    offset += Varint.BufferToNumber(buffer, offset, out index);

                    Type objType;
                    offset += _TypeSet.Get().ToObject(buffer, offset, out objType);
                    var describer = _TypeSet.Get(objType);
                    object value;
                    offset += describer.ToObject(buffer, offset, out value);


                    array[(int)index] = value;
                }

                return offset - begin;
            }
            catch (Exception ex)
            {
                
                throw new DescriberException(typeof(ArrayDescriber), _Type,  "ToObject", ex); ;
            }   
            
        }

        

        
    }
}