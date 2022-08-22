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
        public readonly IReadOnlyCollection<ITypeSymbol> Symbols;
        private readonly EssentialReference _References;

        public SerializableExtractor(EssentialReference references, IEnumerable<TypeSyntax> types)
        {            
            
            this._References = references;
            var set = new HashSet<ITypeSymbol>();
            

            var symbols = from type in types 
                          select _References.Compilation.GetTypeByMetadataName(type.ToString());


            _AddSet(set , symbols);
            Symbols = set;

            Code = string.Join(",", from symbol in Symbols select $"typeof({symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)})");
        }

        private void _AddSet(HashSet<ITypeSymbol> set, IEnumerable<ITypeSymbol> symbols)
        {
            foreach (var symbol in symbols)
            {                
                if (set.Contains(symbol))
                    continue;

                set.Add(symbol);

                if (symbol.Kind == SymbolKind.ArrayType)
                {
                    var ary = symbol as IArrayTypeSymbol;
                    _AddSet(set , new[] { ary.ElementType });
                }
                else if(symbol.TypeKind == TypeKind.Class || symbol.TypeKind == TypeKind.Struct)
                {
                    var type = symbol as INamedTypeSymbol;                    
                    _AddSet(set, type.GetMembers().OfType<IFieldSymbol>().Where(f=>f.IsReadOnly == false && f.IsConst == false && f.DeclaredAccessibility == Accessibility.Public && f.IsStatic == false).Select(f=>f.Type));
                }
            }
            
        }

        

       

       

      


    }
}
