using System.Linq;
using System.Reflection;

namespace Regulus.Remote.Protocol
{
    class RemoteAssemblys
    {
        public readonly Assembly Remote;
        public readonly Assembly Library;
        public readonly Assembly Serialization;

        public RemoteAssemblys()
        {
            var domain = System.AppDomain.CurrentDomain;
            var assemblys = domain.GetAssemblies();

            Serialization = _Query("Regulus.Serialization");
            Remote = _Query("Regulus.Remote");
            Library = _Query("Regulus.Library");            
        }

        Assembly _Query(string name)
        {
            var domain = System.AppDomain.CurrentDomain;
            var assemblys = domain.GetAssemblies();
            var assembly = assemblys.Where(asm => asm.GetName().Name == name).FirstOrDefault();

            if(assembly == null)
            {
                var path = domain.BaseDirectory;
                var fullPath = System.IO.Path.Combine(path, $"{name}.dll");
                try
                {
                    return System.Reflection.Assembly.LoadFile(fullPath);
                }
                catch(System.IO.FileLoadException fle)
                {
                    throw new System.Exception($"找不到指定dll，{fullPath}。");
                }
                catch(System.BadImageFormatException bfe)
                {
                    throw new System.Exception($"dll版本太舊，{fullPath}。");
                }
            }
            return assembly;
        }
        
    }
}
