using System;

namespace Regulus.Remote
{
    public class DynamicSerializer : ISerializable
    {
        private readonly Serialization.Dynamic.Serializer _Serializer;

        public DynamicSerializer()
        {
            _Serializer = new Regulus.Serialization.Dynamic.Serializer();
        }

        

        object ISerializable.Deserialize(Type type, byte[] buffer)
        {
            return _Serializer.BufferToObject(buffer);
        }

        byte[] ISerializable.Serialize(Type type, object instance)
        {
            return _Serializer.ObjectToBuffer(instance);
        }
    }
    
    


}
