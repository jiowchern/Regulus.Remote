using System;
using System.Collections.Generic;
using System.Linq;

namespace Regulus.Serialization.Dynamic
{
    public class DescribersFinder  :IDescribersFinder
    {
        
        private readonly Dictionary<Type, ITypeDescriber> _TypeDescribers;

        
        public DescribersFinder()
        {            
            _TypeDescribers = new Dictionary<Type, ITypeDescriber>();
        }


        ITypeDescriber IDescribersFinder.Get(Type id)
        {
            ITypeDescriber des;
            if (!_TypeDescribers.TryGetValue(id, out des))
            {
                var dess = new TypeIdentifier(id , this).Describers;
                foreach (var typeDescriber in dess)
                {
                    if(!_TypeDescribers.ContainsKey(id))
                        _TypeDescribers.Add(id, typeDescriber);
                }
                return dess.First();
            }
            return des;
        }
    }
}