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
                typeof(Regulus.Remote.Packages.PackageProtocolSubmit),
                typeof(Regulus.Remote.Packages.RequestPackage),
                typeof(Regulus.Remote.Packages.ResponsePackage),
                typeof(Regulus.Remote.Packages.PackageInvokeEvent),
                typeof(Regulus.Remote.Packages.PackageErrorMethod),
                typeof(Regulus.Remote.Packages.PackageReturnValue),
                typeof(Regulus.Remote.Packages.PackageLoadSoulCompile),
                typeof(Regulus.Remote.Packages.PackageLoadSoul),
                typeof(Regulus.Remote.Packages.PackageUnloadSoul),
                typeof(Regulus.Remote.Packages.PackageCallMethod),
                typeof(Regulus.Remote.Packages.PackageRelease),
                typeof(Regulus.Remote.Packages.PackageSetProperty),
                typeof(Regulus.Remote.Packages.PackageSetPropertyDone),
                typeof(Regulus.Remote.Packages.PackageAddEvent),
                typeof(Regulus.Remote.Packages.PackageRemoveEvent),
                typeof(Regulus.Remote.Packages.PackagePropertySoul),
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

        object IInternalSerializable.Deserialize(Regulus.Memorys.Buffer buffer)
        {
            return _Serializer.BufferToObject(buffer);
        }

        Regulus.Memorys.Buffer IInternalSerializable.Serialize(object instance)
        {
            return _Serializer.ObjectToBuffer(instance);
        }
    }
    
    


}
