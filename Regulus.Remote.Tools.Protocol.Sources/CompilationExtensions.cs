
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
            
            var symbols = (from syntaxTree in com.SyntaxTrees
             let model = com.GetSemanticModel(syntaxTree)
             from interfaneSyntax in syntaxTree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>()
             let symbol = model.GetDeclaredSymbol(interfaneSyntax)
             where symbol.IsGenericType == false && symbol.IsAbstract
            select symbol).SelectMany(s => s.AllInterfaces.Union(new[] { s }));

            return new System.Collections.Generic.HashSet<INamedTypeSymbol>(symbols , SymbolEqualityComparer.Default);
        }
    }
}
