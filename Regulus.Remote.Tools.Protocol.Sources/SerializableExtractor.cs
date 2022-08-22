using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Regulus.Remote.Tools.Protocol.Sources
{

    
    using System.Linq;

   

    class SerializableExtractor
    {
        public string Code;

        

        public SerializableExtractor(IEnumerable<TypeSyntax> types)
        {
            
            
            
            

            Code = string.Join(",", from type in types select $"typeof({type.ToFullString()})");
        }


        

       

       

      


    }
}
