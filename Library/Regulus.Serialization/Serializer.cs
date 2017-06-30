using System;
using System.Collections.Generic;
using System.Linq;

using Regulus.Remoting;

namespace Regulus.Serialization
{
    public class Serializer : ISerializer
    {
        private readonly TypeSet _TypeSet;

        public Serializer(params ITypeDescriber[] describers)
        {

            var typeSet = new TypeSet(describers);
            

            foreach (var typeDescriber in describers)
            {
                typeDescriber.SetMap(typeSet);
            }

            _TypeSet = typeSet;
        }

        public Serializer(DescriberBuilder describer_builder) : this(describer_builder.Describers)
        {            
        }

        

        
        public byte[] ObjectToBuffer(object instance )
        {

            try
            {
                if (instance == null)
                {
                    return _NullBuffer();
                }

                var type = instance.GetType();
                var describer = _GetDescriber(type);
                var id = describer.Id;
                var idCount = Varint.GetByteCount((ulong)id);
                var bufferCount = describer.GetByteCount(instance);
                var buffer = new byte[idCount + bufferCount];
                var readCount = Varint.NumberToBuffer(buffer, 0, id);
                describer.ToBuffer(instance, buffer, readCount);
                return buffer;
            }
            catch (DescriberException ex)
            {

                if (instance != null)
                {
                    throw new SystemException(string.Format("ObjectToBuffer {0}", instance.GetType()), ex);                    
                }                    
                else
                {
                    throw new SystemException(string.Format("ObjectToBuffer null"), ex);
                }
            }
            
        }

        private byte[] _NullBuffer()
        {
            var idCount = Varint.GetByteCount(0);
            var buffer = new byte[idCount];
            Varint.NumberToBuffer(buffer, 0, 0);
            return buffer;
        }
        
        public object BufferToObject(byte[] buffer)
        {
            ulong id = 0;
            try
            {
                
                var readIdCount = Varint.BufferToNumber(buffer, 0, out id);
                if (id == 0)
                    return null;

                var describer = _GetDescriber((int)id);
                object instance;
                describer.ToObject(buffer, readIdCount, out instance);
                return instance;
            }
            catch (DescriberException ex)
            {
                var describer = _GetDescriber((int)id);
                if(describer != null)
                    throw new SystemException(string.Format("BufferToObject {0}:{1}", id , describer.Type.FullName), ex);
                else
                {
                    throw new SystemException(string.Format("BufferToObject {0}:unkown", id), ex);
                }
            }
            
        }

        private ITypeDescriber _GetDescriber(int id)
        {

            
            return _TypeSet.GetById(id);
        }

        private ITypeDescriber _GetDescriber(Type type)
        {
            return _TypeSet.GetByType(type);
        }

        byte[] ISerializer.Serialize(object instance )
        {
            return ObjectToBuffer(instance);
        }

        object ISerializer.Deserialize(byte[] buffer)
        {
            return BufferToObject(buffer);
        }

        public bool TryBufferToObject<T>(byte[] buffer, out T pkg)
        {

            pkg = default(T);
            try
            {
                var instance = BufferToObject(buffer);
                pkg = (T) instance;
                return true;
            }
            catch (Exception e)
            {
                Regulus.Utility.Log.Instance.WriteInfo(e.ToString());
            }

            return false;
        }
    }
}



