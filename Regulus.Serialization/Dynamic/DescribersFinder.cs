using System;
using System.Collections.Generic;
using System.Linq;

namespace Regulus.Serialization.Dynamic
{
    public class DescribersFinder : IDescribersFinder
    {

        private readonly Dictionary<Type, ITypeDescriber> _TypeDescribers;
        private readonly StringKeyDescriber _KeyDescriber;


        public DescribersFinder(ITypeFinder type_finder)
        {

            _TypeDescribers = new Dictionary<Type, ITypeDescriber>();
            _KeyDescriber = new StringKeyDescriber(type_finder, this);
        }


        IKeyDescriber IDescribersFinder.Get()
        {
            return _KeyDescriber;
        }

        ITypeDescriber IDescribersFinder.Get(Type id)
        {
            ITypeDescriber des;
            if (!_TypeDescribers.TryGetValue(id, out des))
            {
                IEnumerable<ITypeDescriber> dess = new TypeIdentifier(id, this).Describers;
                foreach (ITypeDescriber typeDescriber in dess)
                {
                    if (!_TypeDescribers.ContainsKey(id))
                        _TypeDescribers.Add(id, typeDescriber);
                }
                return dess.First();
            }
            return des;
        }
    }
}