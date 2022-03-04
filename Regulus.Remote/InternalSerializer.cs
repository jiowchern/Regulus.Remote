using Regulus.Serialization;

namespace Regulus.Remote
{
    public class InternalSerializer : Regulus.Remote.IInternalSerializable
    {
        private readonly Regulus.Serialization.Serializer _Serializer;

        public InternalSerializer()
        {
            var essentialTypes = new System.Type[]
            {
                typeof(Regulus.Remote.PackageProtocolSubmit),
                typeof(Regulus.Remote.RequestPackage),
                typeof(Regulus.Remote.ResponsePackage),
                typeof(Regulus.Remote.PackageInvokeEvent),
                typeof(Regulus.Remote.PackageErrorMethod),
                typeof(Regulus.Remote.PackageReturnValue),
                typeof(Regulus.Remote.PackageLoadSoulCompile),
                typeof(Regulus.Remote.PackageLoadSoul),
                typeof(Regulus.Remote.PackageUnloadSoul),
                typeof(Regulus.Remote.PackageCallMethod),
                typeof(Regulus.Remote.PackageRelease),
                typeof(Regulus.Remote.PackageSetProperty),
                typeof(Regulus.Remote.PackageSetPropertyDone),
                typeof(Regulus.Remote.PackageAddEvent),
                typeof(Regulus.Remote.PackageRemoveEvent),
                typeof(Regulus.Remote.PackagePropertySoul),
                typeof(byte),
                typeof(byte[]),
                typeof(byte[][]),
                typeof(Regulus.Remote.ClientToServerOpCode),
                typeof(Regulus.Remote.ServerToClientOpCode),
                typeof(long),
                typeof(int),
                typeof(string),
                typeof(bool),
                typeof(char),
                typeof(char[])
            };
            _Serializer = new Regulus.Serialization.Serializer(new Regulus.Serialization.DescriberBuilder(essentialTypes).Describers);
        }

        object IInternalSerializable.Deserialize(byte[] buffer)
        {
            return _Serializer.BufferToObject(buffer);
        }

        byte[] IInternalSerializable.Serialize(object instance)
        {
            return _Serializer.ObjectToBuffer(instance);
        }
    }
    
    


}
