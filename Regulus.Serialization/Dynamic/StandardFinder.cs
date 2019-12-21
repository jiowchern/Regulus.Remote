using System;
using System.Linq;

namespace Regulus.Serialization.Dynamic
{
    public class StandardFinder : ITypeFinder
    {
        Type ITypeFinder.Find(string type_name)
        {
            var retType   = Type.GetType(type_name);
            if (retType != null)
                return retType;

            var assembles = AppDomain.CurrentDomain.GetAssemblies();
            foreach(var asm in assembles)
            {
                if (asm.IsDynamic)
                {
                    continue;
                }
                var type = (from t in asm.GetExportedTypes() where t.FullName == type_name select t).FirstOrDefault();
                if (type == null)
                    continue;
                return type;
            }
            return null;
            
        }
    }
}