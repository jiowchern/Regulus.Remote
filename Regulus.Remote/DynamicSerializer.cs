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

        

        object ISerializable.Deserialize(Type type, Regulus.Memorys.Buffer buffer)
        {
            return _Serializer.BufferToObject(buffer);
        }

        Regulus.Memorys.Buffer ISerializable.Serialize(Type type, object instance)
        {
            return _Serializer.ObjectToBuffer(instance);
        }
    }
    
    


}
