using System;


namespace Regulus.Remote.Tests
{
    class Serializer : ISerializable
    {
        
        private readonly Serialization.Dynamic.Serializer _Serializer;
        public readonly ISerializable Serializable ;
        public Serializer()
        {
            _Serializer = new Regulus.Serialization.Dynamic.Serializer();
            Serializable = this;
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
