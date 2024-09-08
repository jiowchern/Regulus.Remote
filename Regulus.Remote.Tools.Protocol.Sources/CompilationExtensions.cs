
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Regulus.Remote.Tools.Protocol.Sources
{
    public static class CompilationExtensions
    {
        public static System.Collections.Generic.IEnumerable<INamedTypeSymbol> FindAllInterfaceSymbol(this Compilation com)
        {
            return com.FindAllInterfaceSymbol(s=>true);
        }
        public static System.Collections.Generic.IEnumerable<INamedTypeSymbol> FindAllInterfaceSymbol(this Compilation com , System.Func<INamedTypeSymbol,bool> additional)
        {            
            var symbols = ( from syntaxTree in com.SyntaxTrees
                            let model = com.GetSemanticModel(syntaxTree)
                            from interfaneSyntax in syntaxTree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>()
                            let symbol = model.GetDeclaredSymbol(interfaneSyntax)
                            where symbol.IsGenericType == false && symbol.IsAbstract && additional(symbol)
                            select symbol).SelectMany(s => s.AllInterfaces.Concat(new[] { s }) );

            return new System.Collections.Generic.HashSet<INamedTypeSymbol>(symbols , SymbolEqualityComparer.Default);
        }

        public static bool AllGhostable(this Compilation compilation, System.Collections.Generic.IEnumerable<TypeSyntax> types)
        {
            foreach (var t in types)
            {
                var typeSymbol = compilation.GetTypeByMetadataName(t.ToString());
                if (typeSymbol == null)
                    return false;
                if (typeSymbol.TypeKind != TypeKind.Interface)
                    return false;
            }
            return true;
        }

        public static bool AllSerializable(this Compilation compilation, System.Collections.Generic.IEnumerable<TypeSyntax> types)
        {
            foreach (var t in types)
            {
                var typeSyntax = t;
                if (typeSyntax is ArrayTypeSyntax arrayType)
                {
                    typeSyntax = arrayType.ElementType;
                }
                var typeName = typeSyntax.ToString();
                var symbol = compilation.GetTypeByMetadataName(typeName);

                if (symbol == null)
                    return false;
                
                if(symbol.IsAbstract)
                    return false;
            }
            return true;
        }

        
        static  bool isSerializableType(int val)
        {
            var begin = (int)SpecialType.System_Boolean;
            var end = (int)SpecialType.System_String;
            return val >= begin && val <= end;
        }
        static bool isSerializableType(ITypeSymbol type)
        {

            
            return isSerializableType((int)type.SpecialType);
        }
    }
}
