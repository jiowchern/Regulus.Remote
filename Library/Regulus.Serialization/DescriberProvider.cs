using System;
using System.Collections.Generic;
using System.Linq;

namespace Regulus.Serialization
{
    internal class DescriberProvider: ITypeDescriberProvider<int>
    {
        

        private readonly IntDescribers _KeyDescribers;
        private readonly TypeDescribers _TypeDescribers;

        public DescriberProvider(params ITypeDescriber[] describers)
        {
            
            _KeyDescribers = new IntDescribers(describers);
            _TypeDescribers = new TypeDescribers(describers);
        }

       
        ITypeDescriberFinder<int> ITypeDescriberProvider<int>.GetKeyFinder()
        {
            return _KeyDescribers;
        }

        ITypeDescriberFinder<Type> ITypeDescriberProvider<int>.GetTypeFinder()
        {
            return _TypeDescribers;
        }

        
    }

    internal class TypeDescribers : ITypeDescriberFinder<Type>
    {
        private readonly Dictionary<Type, ITypeDescriber> _Describers;
        public TypeDescribers(ITypeDescriber[] describers)
        {
            _Describers = new Dictionary<Type, ITypeDescriber>();
            foreach (var typeDescriber in describers)
            {
                typeDescriber.SetMap(this);
                _Describers.Add(typeDescriber.Type ,typeDescriber);
            }


        }

        public ITypeDescriber Get(Type id)
        {
            ITypeDescriber des;
            _Describers.TryGetValue(id, out des);
            return des;
        }
    }

    internal class IntDescribers : ITypeDescriberFinder<int>
    {
        private readonly Dictionary<int, ITypeDescriber> _Describers;
        public IntDescribers(ITypeDescriber[] describers)
        {
            _Describers = new Dictionary<int, ITypeDescriber>();

            for (int i = 0; i < describers.Length; i++)
            {
                _Describers.Add(i+1 , describers[i]);
            }
        }

        ITypeDescriber ITypeDescriberFinder<int>.Get(int id)
        {
            ITypeDescriber des ;
            _Describers.TryGetValue(id, out des);
            return des;
        }
    }
}