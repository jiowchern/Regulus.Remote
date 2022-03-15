using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

namespace Regulus.Remote.Tools.Protocol.Sources
{
    public class InterfaceInheritor
    {
        
        private readonly InterfaceDeclarationSyntax _Base;
        public readonly ArrowExpressionClauseSyntax Expression;

        public InterfaceInheritor(InterfaceDeclarationSyntax @base) :this(@base , SyntaxFactory.ArrowExpressionClause(SyntaxFactory.ThrowExpression(SyntaxFactory.ParseExpression("new System.NotImplementedException(\"\")"))) )
        {
            
        }
        public InterfaceInheritor(InterfaceDeclarationSyntax @base, ArrowExpressionClauseSyntax expression)
        {
            Expression = expression;
            this._Base = @base;
        }

        public ClassDeclarationSyntax Inherite(ClassDeclarationSyntax class_syntax)
        {
            var interfaceDeclaration = _Base;
            
            var exc = Expression;
            var explicitInterfaceSpecifier = SyntaxFactory.ExplicitInterfaceSpecifier(SyntaxFactory.ParseName(interfaceDeclaration.Identifier.ToString()));

            var classSyntax = class_syntax;
            
            var nodes = interfaceDeclaration.DescendantNodes().OfType<MemberDeclarationSyntax>();

            var newNodes = nodes;
            

            foreach (var member in newNodes.OfType<MethodDeclarationSyntax>())
            {
                var node = member;

                node = node.WithExplicitInterfaceSpecifier(explicitInterfaceSpecifier);
                node = node.WithExpressionBody(exc);
                classSyntax = classSyntax.AddMembers(node);
            }

            foreach (var member in newNodes.OfType<PropertyDeclarationSyntax>())
            {
                var node = member;
                node = node.WithExplicitInterfaceSpecifier(explicitInterfaceSpecifier);

                var accessors = from a in node.AccessorList.Accessors
                                select a.WithExpressionBody(exc);

                node = node.WithAccessorList(SyntaxFactory.AccessorList(SyntaxFactory.List(accessors)));

                classSyntax = classSyntax.AddMembers(node);

            }

            foreach (var member in newNodes.OfType<EventFieldDeclarationSyntax>())
            {
                var node = member;
                var eve = SyntaxFactory.EventDeclaration(node.Declaration.Type, node.Declaration.Variables[0].Identifier);
                
                eve = eve.WithExplicitInterfaceSpecifier(explicitInterfaceSpecifier);
                var accessors = SyntaxFactory.AccessorList();
                accessors = accessors.AddAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.AddAccessorDeclaration).WithExpressionBody(exc));
                accessors = accessors.AddAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.RemoveAccessorDeclaration).WithExpressionBody(exc));
                eve = eve.WithAccessorList(accessors);
                classSyntax = classSyntax.AddMembers(eve);
            }
           

            foreach (var member in newNodes.OfType<IndexerDeclarationSyntax>())
            {
                var node = member;
                node = node.WithExplicitInterfaceSpecifier(explicitInterfaceSpecifier);

                var accessors = from a in node.AccessorList.Accessors
                                select a.WithExpressionBody(exc);
                node = node.WithAccessorList(SyntaxFactory.AccessorList(SyntaxFactory.List(accessors)));

                classSyntax = classSyntax.AddMembers(node);                
            }


            return classSyntax;
        }
 
    }


}