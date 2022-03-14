using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Regulus.Remote.Tools.Protocol.Sources.Extensions
{
    public static class SyntaxExtensions
    {
        public static SyntaxNode ToSyntax(this INamedTypeSymbol symbol)
        {
            
            var syntax = SyntaxFactory.InterfaceDeclaration(symbol.Name);
            



            foreach (var member in symbol.GetMembers())
            {
                MemberDeclarationSyntax memberSyntax = null;
                if (member is IMethodSymbol methodSymbol)
                {

                    var retType = _GetTypeName(methodSymbol.ReturnType);
                    var method = SyntaxFactory.MethodDeclaration(retType, methodSymbol.Name);
                    var list = SyntaxFactory.ParameterList();
                    
                    foreach (var parameter in methodSymbol.Parameters)
                    {
                        var p = SyntaxFactory.Parameter(SyntaxFactory.Identifier(parameter.Name));
                        p = p.WithType(_GetTypeName(parameter.Type));
                        list = list.AddParameters(p);
                        
                    }
                    method = method.WithParameterList(list);
                    memberSyntax = method;
                }
                
                syntax = syntax.AddMembers(memberSyntax);
            }

            var root = SyntaxFactory.CompilationUnit();

            var namespaceSymbol = symbol.ContainingNamespace;
            if (namespaceSymbol != null)
            {
                var namespaceSyntax = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(namespaceSymbol.ToDisplayString()));
                namespaceSyntax = namespaceSyntax.AddMembers(syntax);
                root = root.AddMembers(namespaceSyntax);
            }
            else
            {
                root = root.AddMembers(syntax);
            }


            return root;
        }

        private static TypeSyntax _GetTypeName(ITypeSymbol symbol)
        {
            return SyntaxFactory.ParseTypeName(symbol.ToDisplayString());
        }

    }
}