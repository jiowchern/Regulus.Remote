using System.Linq;
using System.Reflection;

namespace Regulus.Remote.Protocol
{
    class RefAssemblyFinder
    {
        public readonly Assembly[] Assemblys;


        public RefAssemblyFinder(System.Collections.Generic.IEnumerable<System.Type> types)
        {
            var assmeblys = new System.Collections.Generic.HashSet<Assembly>();
            foreach(var t1 in types)
            {
                _Add(assmeblys, t1.Assembly);
                
                foreach (var t2 in new Regulus.Remote.Protocol.TypeDisintegrator(t1).Types)
                {
                    var assembly = t2.Assembly;
                    
                   
                    _Add(assmeblys, assembly);
                }
            }

          

            Assemblys = assmeblys.ToArray();
        }

        private static void _Add(System.Collections.Generic.HashSet<Assembly> assmeblys, Assembly assembly)
        {
            if (assembly.IsDynamic)
                return;

            //if (assembly.GetName().Name == "System.Private.CoreLib")
             //   return;

            if (assmeblys.Add(assembly))
            {
                Regulus.Utility.Log.Instance.WriteInfo($"Ref Assembly {assembly.Location} .");
            }

            foreach(var a in assembly.GetReferencedAssemblies())
            {
                
                if (assmeblys.Add(Assembly.Load(a)))
                {
                    Regulus.Utility.Log.Instance.WriteInfo($"Ref Assembly {assembly.Location}.");
                }
            }
        }

     

       
        
    }
}
