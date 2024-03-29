﻿
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace Regulus.Remote.Tools.Protocol.Sources.Extensions
{
    public static class SyntaxExtensions
    {
        public static bool AnyNull(params SyntaxNode[] nodes)
        {
            
                
            return nodes.Any(n => n == null);
        }

        public static System.Collections.Generic.IEnumerable<SyntaxNode> GetParentPathAndSelf(this SyntaxNode syntax)
        {            
            while(syntax != null)
            {
                yield return syntax;
                syntax = syntax.Parent;
            }
        }
        public static string GetNamePath(this BaseTypeDeclarationSyntax syntax)
        {
            
            var names = new System.Collections.Generic.Stack<string>();

            NamespaceDeclarationSyntax @namespace = default;
            while (syntax != null)
            {
                names.Push(syntax.Identifier.ValueText);                

                if (syntax.Parent is BaseTypeDeclarationSyntax type)
                    syntax = type;
                else if(syntax.Parent is NamespaceDeclarationSyntax ns)
                {
                    @namespace = ns;
                    break;
                }
                else
                {
                    break;
                }
            }

            while(@namespace !=null)
            {
                names.Push(@namespace.Name.ToString());

                if(@namespace.Parent is NamespaceDeclarationSyntax ns)
                {
                    @namespace = ns; 
                }
                else
                {
                    break;
                }
                
                  
            }
            
            return string.Join(".", names);
        }

        public static InterfaceDeclarationSyntax ToInferredInterface(this INamedTypeSymbol symbol)
        {
            
            var syntax = SyntaxFactory.InterfaceDeclaration(symbol.Name);

            foreach (var member in symbol.GetMembers())
            {
                MemberDeclarationSyntax memberSyntax = null;
                if (member is IMethodSymbol methodSymbol && methodSymbol.MethodKind == MethodKind.Ordinary)
                {                    
                    memberSyntax = _ToMethod(methodSymbol);                    
                }
                else if (member is IPropertySymbol propertySymbol)
                {
                    if(propertySymbol.IsIndexer)
                    {
                        memberSyntax = _ToIndexer(propertySymbol);
                    }
                    else
                    {
                        memberSyntax = _ToProperty(propertySymbol);
                    }
                    
                }
                else if (member is IEventSymbol eventSymbol)
                {
                    memberSyntax = _ToEvent(eventSymbol);
                }
                else
                {
                    continue;
                }

                syntax = syntax.AddMembers(memberSyntax);
            }

            var root = SyntaxFactory.CompilationUnit();

            var namespaceSymbol = symbol.ContainingNamespace;
            if (namespaceSymbol.IsGlobalNamespace)
            {
                root = root.AddMembers(syntax);
            }
            else
            {
                var namespaceSyntax = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(namespaceSymbol.ToDisplayString()));
                namespaceSyntax = namespaceSyntax.AddMembers(syntax);
                root = root.AddMembers(namespaceSyntax);                
            }

            
            return root.DescendantNodes().OfType<InterfaceDeclarationSyntax>().First();
        }

       

        private static MemberDeclarationSyntax _ToEvent(IEventSymbol event_symbol)
        {
            var type = _GetTypeSyntax(event_symbol.Type);

            
            SeparatedSyntaxList<VariableDeclaratorSyntax> vars = default;
            vars = vars.Add(SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(event_symbol.Name)));
            var varibable = SyntaxFactory.VariableDeclaration(type , vars );

            
            var member = SyntaxFactory.EventFieldDeclaration(varibable);

            return member;

        }
        private static MemberDeclarationSyntax _ToIndexer(IPropertySymbol property_symbol)
        {
            var type = _GetTypeSyntax(property_symbol.Type);            
            var property = SyntaxFactory.IndexerDeclaration(type);

            if (property_symbol.SetMethod != null)
            {
                var ad = SyntaxKind.SetAccessorDeclaration;
                AccessorDeclarationSyntax access = _AccessorSyntax(ad);
                property = property.AddAccessorListAccessors(access);
            }
            if (property_symbol.GetMethod != null)
            {
                var ad = SyntaxKind.GetAccessorDeclaration;
                AccessorDeclarationSyntax access = _AccessorSyntax(ad);
                property = property.AddAccessorListAccessors(access);
            }
            return property;
        }
        private static PropertyDeclarationSyntax _ToProperty(IPropertySymbol property_symbol)
        {
            var type = _GetTypeSyntax(property_symbol.Type);
            var property = SyntaxFactory.PropertyDeclaration(type, property_symbol.Name);

            if(property_symbol.SetMethod != null)
            {
                var ad = SyntaxKind.SetAccessorDeclaration;
                AccessorDeclarationSyntax access = _AccessorSyntax(ad);
                property = property.AddAccessorListAccessors(access);
            }
            if (property_symbol.GetMethod != null)
            {
                var ad = SyntaxKind.GetAccessorDeclaration;
                AccessorDeclarationSyntax access = _AccessorSyntax(ad);
                property = property.AddAccessorListAccessors(access);
            }
            return property;
        }

        private static AccessorDeclarationSyntax _AccessorSyntax(SyntaxKind ad)
        {
            return SyntaxFactory.AccessorDeclaration(ad);
        }

        private static MethodDeclarationSyntax _ToMethod(IMethodSymbol methodSymbol)
        {
            var retType = _GetTypeSyntax(methodSymbol.ReturnType);
            var method = SyntaxFactory.MethodDeclaration(retType, methodSymbol.Name);
            var list = SyntaxFactory.ParameterList();

            foreach (var parameter in methodSymbol.Parameters)
            {
                var p = SyntaxFactory.Parameter(SyntaxFactory.Identifier(parameter.Name));

                
                if(parameter.RefKind == RefKind.Out)
                {
                    p = p.AddModifiers(SyntaxFactory.Token(SyntaxKind.OutKeyword));
                }
                else if (parameter.RefKind == RefKind.In)
                {
                    p = p.AddModifiers(SyntaxFactory.Token(SyntaxKind.InKeyword));
                }
                else if (parameter.RefKind == RefKind.Ref)
                {
                    p = p.AddModifiers(SyntaxFactory.Token(SyntaxKind.RefKeyword));
                }
                else if (parameter.RefKind == RefKind.RefReadOnly)
                {
                    p = p.AddModifiers(SyntaxFactory.Token(SyntaxKind.RefKeyword));
                    p = p.AddModifiers(SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword));
                }
                p = p.WithType(_GetTypeSyntax(parameter.Type));
                list = list.AddParameters(p);

            }
            method = method.WithParameterList(list);
            return method;
        }

        private static TypeSyntax _GetTypeSyntax(ITypeSymbol symbol)
        {
            if (symbol.SpecialType == SpecialType.System_Void)
            {
                return SyntaxFactory.ParseTypeName("void");
            }
           
            
            var syntax = SyntaxFactory.ParseTypeName(symbol.ToDisplayString(FullSpreadSymbolDisplayFormat.Default));
            return syntax;
        }
    }
}