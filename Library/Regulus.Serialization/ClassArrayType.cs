using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace Regulus.Serialization
{

    public class ClassArrayType<T> : ClassArrayType
    {
        public ClassArrayType(int id) : base(id, typeof (T[]))
        {
            
        }
    }
    public class ClassArrayType : ITypeDescriber 
    {
        private readonly int _Id;

        private readonly Type _Type;

        private ITypeDescriber[] _Describers;

        private object _Default;
        private object _DefaultElement;

        private Type _ElementType;

        public ClassArrayType(int id , Type type)
        {
            _Default = null;

            _ElementType = type.GetElementType();
            if(!_ElementType.IsClass)
                _DefaultElement = Activator.CreateInstance(_ElementType);
            else
            {
                _DefaultElement = null;
            }
            _Id = id;
            _Type = type;
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


            var lenCount = Serializer.Varint.GetByteCount(set.TotalLength);
            var validCount = Serializer.Varint.GetByteCount(set.ValidLength);

            var describer = _GetDescriber(_ElementType);
            var instanceCount = 0;
            for (int i = 0; i < set.ValidObjects.Length; i++)
            {
                var index = set.ValidObjects[i].Index;
                var obj = set.ValidObjects[i].Object;
                instanceCount += Serializer.Varint.GetByteCount(index);
                instanceCount += describer.GetByteCount(obj);
            }

            

            return instanceCount + lenCount + validCount;
        }

        int ITypeDescriber.ToBuffer(object instance, byte[] buffer, int begin)
        {

            var set = _GetSet(instance);
            var offset = begin;
            offset += Serializer.Varint.NumberToBuffer(buffer, offset, set.TotalLength);
            offset += Serializer.Varint.NumberToBuffer(buffer, offset, set.ValidLength);

            var describer = _GetDescriber(_ElementType);
            for (int i = 0; i < set.ValidObjects.Length; i++)
            {
                var index = set.ValidObjects[i].Index;
                var obj = set.ValidObjects[i].Object;
                offset += Serializer.Varint.NumberToBuffer(buffer, offset, index);
                offset += describer.ToBuffer(obj, buffer, offset);
            }
            
            return offset - begin;
        }

        int ITypeDescriber.ToObject(byte[] buffer, int begin, out object instnace)
        {
            
            var offset = begin;
            ulong count;
            offset += Serializer.Varint.BufferToNumber(buffer, offset, out count);
            var array = Activator.CreateInstance(_Type , (int)count) as IList;
            instnace = array;

            ulong validCount;
            offset += Serializer.Varint.BufferToNumber(buffer, offset, out validCount);

            var describer = _GetDescriber(_ElementType);
            for (var i = 0UL; i < validCount; i++)
            {
                var index = 0LU;

                offset += Serializer.Varint.BufferToNumber(buffer, offset, out index);

                object value;
                offset += describer.ToObject(buffer, offset, out value);

                
                array[(int)index] = value ;
            }

            return offset - begin;
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