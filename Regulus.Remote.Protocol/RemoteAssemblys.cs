using System.Linq;
using System.Reflection;

namespace Regulus.Remote.Protocol
{
    class BaseRemoteAssemblys
    {
        public readonly Assembly[] Assemblys;


        public BaseRemoteAssemblys(System.Collections.Generic.IEnumerable<System.Type> types)
        {
            var assmeblys = new System.Collections.Generic.HashSet<Assembly>();
            foreach(var t1 in types)
            {
                _Add(assmeblys, t1.Assembly);
                
                foreach (var t2 in new Regulus.Remote.Protocol.TypeDisintegrator(t1).Types)
                {
                    var assembly = t2.Assembly;
                    
                    /*if (assembly.GetName().Name == "System.Private.CoreLib")
                        continue;*/
                    _Add(assmeblys, assembly);
                }
            }

            _Add(assmeblys, _Find("Regulus.Serialization"));
            _Add(assmeblys, _Find("Regulus.Utility"));
            _Add(assmeblys, _Find("Regulus.Remote"));
            _Add(assmeblys, _Find("System.Runtime"));
            _Add(assmeblys, _Find("System.Linq.Expressions"));
            _Add(assmeblys, _Find("System.Collections"));
          
            

            Assemblys = assmeblys.ToArray();
        }

        private static void _Add(System.Collections.Generic.HashSet<Assembly> assmeblys, Assembly assembly)
        {
            if (assembly.IsDynamic)
                return;

            if (assmeblys.Add(assembly))
            {
                Regulus.Utility.Log.Instance.WriteInfo($"Found Assembly {assembly.GetName()}.");
            }

            foreach(var a in assembly.GetReferencedAssemblies())
            {
                
                if (assmeblys.Add(Assembly.Load(a)))
                {
                    Regulus.Utility.Log.Instance.WriteInfo($"Found Assembly {assembly.GetName()}.");
                }
            }
        }

        Assembly _Find(string name)
        {
            var domain = System.AppDomain.CurrentDomain;
            var assemblys = domain.GetAssemblies();
            var assembly = assemblys.Where(asm => asm.GetName().Name == name).SingleOrDefault();

            if (assembly == null)
            {
                var path = domain.BaseDirectory;
                var fullPath = System.IO.Path.Combine(path, $"{name}.dll");
                try
                {
                    return System.Reflection.Assembly.LoadFile(fullPath);
                }
                catch (System.IO.FileLoadException fle)
                {
                    throw new System.Exception($"找不到指定dll，{fullPath}。");
                }
                catch (System.BadImageFormatException bfe)
                {
                    throw new System.Exception($"dll版本太舊，{fullPath}。");
                }
            }
            return assembly;
        }

       
        
    }
}
