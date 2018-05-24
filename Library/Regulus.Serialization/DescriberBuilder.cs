using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Regulus.Serialization
{
    public class DescriberBuilder
    {
        public readonly DescriberProvider Describers;
        public DescriberBuilder(params Type[] types)
        {
            Describers = _BuildDescribers(types);
        }
        DescriberProvider _BuildDescribers(params Type[] types)
        {
            
            var describers = new List<ITypeDescriber>();
            foreach (var type in types)
            {
                var identifier = new TypeIdentifier(type );
                describers.Add(identifier.Describer);
               
            }

            return new DescriberProvider(describers.ToArray()) ;
        }
      
    }
}
