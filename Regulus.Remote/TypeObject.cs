using System;

namespace Regulus.Remote
{
    public class TypeObject
    {
        public readonly Type Type;
        public readonly object Instance;
        public TypeObject(Type type,object instance)
        {
            Type = type;
            Instance = instance;
        }
    }
}
