using System;
using System.Linq;
namespace Regulus.Remote
{

    public class Serializer : ISerializable
    {
        private readonly Serialization.Serializer _Serializer;

        public Serializer(System.Collections.Generic.IEnumerable<System.Type> types)
        {
            _Serializer = new Regulus.Serialization.Serializer(new Regulus.Serialization.DescriberBuilder(types.ToArray()).Describers);
        }
        object ISerializable.Deserialize(Type type, byte[] buffer)
        {
            return _Serializer.BufferToObject(buffer);
        }

        Regulus.Memorys.Buffer ISerializable.Serialize(Type type, object instance)
        {
            return _Serializer.ObjectToBuffer(instance);    
        } 
    }




}
