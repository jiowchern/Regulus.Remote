using System;
using System.Collections.Generic;

namespace Regulus.Serialization.Dynamic
{
    public class DescriberFinder : ITypeDescriberFinder<string> , ITypeDescriberFinder<Type>
    {
        private readonly ITypeFinder _Finder;
        private readonly Dictionary<string, ITypeDescriber> _StringDescribers;
        private readonly Dictionary<Type, ITypeDescriber> _TypeDescribers;
        public DescriberFinder(ITypeFinder finder)
        {
            _Finder = finder;
            _StringDescribers = new Dictionary<string, ITypeDescriber>();
            _TypeDescribers = new Dictionary<Type, ITypeDescriber>();
        }

        

        ITypeDescriber ITypeDescriberFinder<string>.Get(string id)
        {
            ITypeDescriber des;
            if (!_StringDescribers.TryGetValue(id, out des))
            {
                var type = _Finder.Find(id);
                if (type == null)
                    return null;
                des = new TypeIdentifier(type,0).Describer;
                des.SetMap(this);
                _StringDescribers.Add(type.FullName , des);
            }
            return des;
        }

        ITypeDescriber ITypeDescriberFinder<Type>.Get(Type id)
        {
            ITypeDescriber des;
            if (!_TypeDescribers.TryGetValue(id, out des))
            {
                des = new TypeIdentifier(id, 0).Describer;
                des.SetMap(this);
                _TypeDescribers.Add(id , des);
            }
            return des;
        }
    }
}