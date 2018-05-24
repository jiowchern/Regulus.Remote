using System;
using System.Collections.Generic;

namespace Regulus.Serialization
{
    public class DescribersFinder : IDescribersFinder
    {
        private readonly Dictionary<Type, ITypeDescriber> _Describers;
        public DescribersFinder(ITypeDescriber[] describers)
        {
            _Describers = new Dictionary<Type, ITypeDescriber>();
            foreach (var typeDescriber in describers)
            {
                
                _Describers.Add(typeDescriber.Type ,typeDescriber);
            }
            
        }

        ITypeDescriber IDescribersFinder.Get(Type id)
        {
            ITypeDescriber des;
            _Describers.TryGetValue(id, out des);
            return des;
        }
    }
}