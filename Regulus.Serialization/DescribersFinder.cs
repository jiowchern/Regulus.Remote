using System;
using System.Collections.Generic;
using System.Linq;

namespace Regulus.Serialization
{
    public class DescribersFinder : IDescribersFinder
    {
        private readonly Dictionary<Type, ITypeDescriber> _Describers;
        public readonly IKeyDescriber KeyDescriber;
        public DescribersFinder(params Type[] types)
        {
            List<ITypeDescriber> describers = new List<ITypeDescriber>();
            foreach (Type type in types)
            {
                TypeIdentifier identifier = new TypeIdentifier(type, this);
                describers.AddRange(identifier.Describers);
            }

            _Describers = new Dictionary<Type, ITypeDescriber>();
            foreach (ITypeDescriber typeDescriber in describers)
            {
                if (!_Describers.ContainsKey(typeDescriber.Type))
                    _Describers.Add(typeDescriber.Type, typeDescriber);
            }

            KeyDescriber = new IntKeyDescriber(_Describers.Values.ToArray());
        }


        IKeyDescriber IDescribersFinder.Get()
        {
            return KeyDescriber;
        }

        ITypeDescriber IDescribersFinder.Get(Type id)
        {
            ITypeDescriber des;
            _Describers.TryGetValue(id, out des);
            return des;
        }
    }
}