using System;
using System.Collections.Generic;

namespace Regulus.Serialization.Dynamic
{
    public class DescribersFinder  :IDescribersFinder
    {
        private readonly ITypeFinder _Finder;
        private readonly Dictionary<string, ITypeDescriber> _StringDescribers;
        private readonly Dictionary<Type, ITypeDescriber> _TypeDescribers;

        
        public DescribersFinder(ITypeFinder finder)
        {
            _Finder = finder;
            _StringDescribers = new Dictionary<string, ITypeDescriber>();
            _TypeDescribers = new Dictionary<Type, ITypeDescriber>();
        }

        

        public ITypeDescriber Get(string id)
        {
            ITypeDescriber des;
            if (!_StringDescribers.TryGetValue(id, out des))
            {
                var type = _Finder.Find(id);
                if (type == null)
                    return null;
                des = new TypeIdentifier(type).Describer;
                des.SetFinder(this);
                _StringDescribers.Add(type.FullName , des);
            }
            return des;
        }

        public ITypeDescriber Get(Type id)
        {
            ITypeDescriber des;
            if (!_TypeDescribers.TryGetValue(id, out des))
            {
                des = new TypeIdentifier(id).Describer;
                des.SetFinder(this);
                _TypeDescribers.Add(id , des);
            }
            return des;
        }
    }
}