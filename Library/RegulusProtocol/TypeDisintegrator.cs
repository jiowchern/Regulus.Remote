using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Regulus.Serialization;

namespace Regulus.Protocol
{
    public class TypeDisintegrator
    {
        public readonly Type Main;

        public TypeDisintegrator(Type type)
        {

            
            var id = new TypeIdentifier(type);

            if (id.Type == TypeIdentifier.TYPE.STRING)
            {
                    //todo : 分割出可續裂化物建
            }
        }
    }
}
