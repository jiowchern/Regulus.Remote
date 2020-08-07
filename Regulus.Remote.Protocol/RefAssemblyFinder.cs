using System.Linq;
using System.Reflection;

namespace Regulus.Remote.Protocol
{
    class RefAssemblyFinder
    {
        public readonly Assembly[] Assemblys;


        public RefAssemblyFinder(System.Collections.Generic.IEnumerable<System.Type> types)
        {
            System.Collections.Generic.HashSet<Assembly> assmeblys = new System.Collections.Generic.HashSet<Assembly>();
            foreach (System.Type t1 in types)
            {
                _Add(assmeblys, t1.Assembly);

                foreach (System.Type t2 in new Regulus.Remote.Protocol.TypeDisintegrator(t1).Types)
                {
                    Assembly assembly = t2.Assembly;


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

            foreach (AssemblyName a in assembly.GetReferencedAssemblies())
            {

                if (assmeblys.Add(Assembly.Load(a)))
                {
                    Regulus.Utility.Log.Instance.WriteInfo($"Ref Assembly {assembly.Location}.");
                }
            }
        }





    }
}
