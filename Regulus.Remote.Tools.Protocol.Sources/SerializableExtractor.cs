using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Regulus.Remote.Tools.Protocol.Sources
{
    using System;
    using System.Linq;

   

    class SerializableExtractor
    {
        public string Code;

        

        public SerializableExtractor(Compilation compilation, IEnumerable<TypeSyntax> types)
        {
            var serializable = new System.Collections.Generic.List<ITypeSymbol>();
            var ttt = types.ToArray();
            foreach (var t in types)
            {
                var type = t;                
                var typeName = type.ToFullString();
                var typeSymbol = compilation.GetTypeByMetadataName(typeName);                
                
                _Collect(typeSymbol , serializable);

                if(type is ArrayTypeSyntax arrayType)
                {
                    var elementType = arrayType.ElementType;
                    var elementTypeName = elementType.ToFullString();
                    var elementTypeSymbol = compilation.GetTypeByMetadataName(elementTypeName);
                    if(elementTypeSymbol == null)
                    {
                           continue;
                    }
                    _Collect(elementTypeSymbol , serializable);
                    var arraySymbol = compilation.CreateArrayTypeSymbol(elementTypeSymbol);
                    _Collect(arraySymbol , serializable);
                }
            }

            Code = string.Join(",", from type in serializable select $"typeof({type.ToDisplayString()})");
        }

        private void _Collect(ITypeSymbol typeSymbol, List<ITypeSymbol> serializables)
        {
            if (typeSymbol == null)
                return;
            if (serializables.Contains(typeSymbol))
                return;
            serializables.Add(typeSymbol);

            if (typeSymbol is IArrayTypeSymbol arraySymbol)
            {
                _Collect(arraySymbol.ElementType , serializables);                
            }

            var fields = typeSymbol.GetMembers().OfType<IFieldSymbol>().Where(f => f.DeclaredAccessibility == Accessibility.Public);
            foreach (var field in fields)
            {
                _Collect(field.Type , serializables);
            }
        }
       
    }
}
