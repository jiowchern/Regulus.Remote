using System;
using System.Collections.Generic;

namespace Regulus.Serialization
{
    public class IntDescribers 
    {
        private readonly Dictionary<Type, int> _Ids;
        private readonly Dictionary<int, ITypeDescriber> _Describers;
        public IntDescribers(ITypeDescriber[] describers)
        {
            _Describers = new Dictionary<int, ITypeDescriber>();
            _Ids = new Dictionary<Type, int>();
            for (int i = 0; i < describers.Length; i++)
            {
                var id = i + 1;
                var des = describers[i];
                _Describers.Add(id ,des );
                _Ids.Add(des.Type , id);
            }
        }

        public ITypeDescriber Get(int id)
        {
            ITypeDescriber des ;
            _Describers.TryGetValue(id, out des);
            return des;
        }

        public int Get(Type type)
        {
            int id;
            _Ids.TryGetValue(type, out id);
            return id;
        }
    }
}