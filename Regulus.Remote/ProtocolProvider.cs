using System.Linq;

namespace Regulus.Remote.Protocol
{
    public static class ProtocolProvider
    {
        public static System.Collections.Generic.IEnumerable<Regulus.Remote.IProtocol> Create(System.Reflection.Assembly protocol_assembly)
        {
            System.Type[] types = protocol_assembly.GetExportedTypes();
            var protocolTypes = types.Where(type => type.GetInterface(nameof(Regulus.Remote.IProtocol)) != null);
            foreach (var type in protocolTypes)
            {
                yield return System.Activator.CreateInstance(type) as Regulus.Remote.IProtocol;
            }
            
        }

        public static System.Collections.Generic.IEnumerable<System.Type> GetProtocols()
        {
            return System.AppDomain.CurrentDomain.GetAssemblies().Where(a => a.IsDynamic == false).SelectMany(assembly => assembly.GetExportedTypes().Where(type => typeof(Regulus.Remote.IProtocol).IsAssignableFrom(type) && typeof(Regulus.Remote.IProtocol) != type).Select(t => t));
        }


    }
}
