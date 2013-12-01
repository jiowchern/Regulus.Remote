using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game
{
    class Resource<T,Key,Value> : Regulus.Utility.Singleton<T> where T :class,new ()
    {
        protected Dictionary<Key, Value> _Resource;
        public Resource()
        {
            _Resource = new Dictionary<Key, Value>();
        }

        public Value Find(Key key)
        { 
            Value value;
            if (_Resource.TryGetValue(key, out value))
            {
                return value;
            }
            return default(Value);
        }
    }
}
