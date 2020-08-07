using System;
using System.Linq;

namespace Regulus.Serialization.Dynamic
{
    public class StandardFinder : ITypeFinder
    {
        Type ITypeFinder.Find(string type_name)
        {
            Type retType = Type.GetType(type_name);
            if (retType != null)
                return retType;

            System.Reflection.Assembly[] assembles = AppDomain.CurrentDomain.GetAssemblies();
            foreach (System.Reflection.Assembly asm in assembles)
            {
                if (asm.IsDynamic)
                {
                    continue;
                }
                Type type = (from t in asm.GetExportedTypes() where t.FullName == type_name select t).FirstOrDefault();
                if (type == null)
                    continue;
                return type;
            }
            return null;

        }
    }
}