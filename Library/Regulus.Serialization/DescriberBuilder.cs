using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Regulus.Serialization.Dynamic;

namespace Regulus.Serialization
{
    public class DescriberBuilder
    {
        public readonly DescriberProvider Describers;
        public DescriberBuilder(params Type[] types)
        {
            Describers = _BuildDescribers(types);
        }

        public DescriberBuilder(ITypeFinder type_finder)
        {
            Describers = _BuildDescribers(type_finder);
        }

        DescriberProvider _BuildDescribers(ITypeFinder type_finder)
        {
            var describersFinder = new Dynamic.DescribersFinder(type_finder);
            return new DescriberProvider(new StringKeyDescriber(type_finder , describersFinder), describersFinder);
        }
        DescriberProvider _BuildDescribers(params Type[] types)
        {
            
            var describers = new List<ITypeDescriber>();
            foreach (var type in types)
            {
                var identifier = new TypeIdentifier(type );
                describers.Add(identifier.Describer);
               
            }
            var set = describers.ToArray();
            return new DescriberProvider(new IntKeyDescriber(set),new DescribersFinder(set) ) ;
        }
      
    }
}
