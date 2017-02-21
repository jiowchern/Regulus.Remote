using System;
using System.Collections.Generic;

namespace Regulus.Remoting
{
    /// <summary>
    /// 
    /// </summary>
    public class GPIProvider 
    {
        private readonly Dictionary<Type, Type> _Types;

        public GPIProvider(Dictionary<Type, Type> types)
        {
            _Types = types;
        }
        public Type Find(Type ghost_base_type)
        {
            if (_Types.ContainsKey(ghost_base_type))
            {                
                return _Types[ghost_base_type];              
            }
            return null;
        }
    }
}