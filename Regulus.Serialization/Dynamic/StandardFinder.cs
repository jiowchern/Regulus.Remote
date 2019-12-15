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


            return (from asm in AppDomain.CurrentDomain.GetAssemblies()
                let type = (from t in asm.GetTypes() where t.FullName == type_name select t).FirstOrDefault()
                where type != null
                select type).FirstOrDefault();
        }
    }
}