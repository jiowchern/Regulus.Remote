using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Regulus.Remote.Tools.Protocol.Sources.Extensions;
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
            var namePath = interfaceDeclaration.GetNamePath();

            var expression = Expression;
            
            var explicitInterfaceSpecifier = SyntaxFactory.ExplicitInterfaceSpecifier(SyntaxFactory.ParseName(namePath));

            var classSyntax = class_syntax;
            var bases = classSyntax.BaseList ?? SyntaxFactory.BaseList();
            bases = bases.AddTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(namePath)));
            classSyntax = classSyntax.WithBaseList(bases);

            var nodes = interfaceDeclaration.DescendantNodes().OfType<MemberDeclarationSyntax>();

            foreach (var member in nodes.OfType<MethodDeclarationSyntax>())
            {
                var node = member;

                node = node.WithExplicitInterfaceSpecifier(explicitInterfaceSpecifier);
                node = node.WithExpressionBody(expression);
                classSyntax = classSyntax.AddMembers(node);
            }

            foreach (var member in nodes.OfType<PropertyDeclarationSyntax>())
            {
                var node = member;
                node = node.WithExplicitInterfaceSpecifier(explicitInterfaceSpecifier);

                var accessors = from a in node.AccessorList.Accessors
                                select a.WithExpressionBody(expression);

                node = node.WithAccessorList(SyntaxFactory.AccessorList(SyntaxFactory.List(accessors)));

                classSyntax = classSyntax.AddMembers(node);

            }

            foreach (var member in nodes.OfType<EventFieldDeclarationSyntax>())
            {
                var node = member;
                var eventDeclaration = SyntaxFactory.EventDeclaration(node.Declaration.Type, node.Declaration.Variables[0].Identifier);
                
                eventDeclaration = eventDeclaration.WithExplicitInterfaceSpecifier(explicitInterfaceSpecifier);
                var accessors = SyntaxFactory.AccessorList();
                accessors = accessors.AddAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.AddAccessorDeclaration).WithExpressionBody(expression));
                accessors = accessors.AddAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.RemoveAccessorDeclaration).WithExpressionBody(expression));
                eventDeclaration = eventDeclaration.WithAccessorList(accessors);
                classSyntax = classSyntax.AddMembers(eventDeclaration);
            }
           

            foreach (var member in nodes.OfType<IndexerDeclarationSyntax>())
            {
                var node = member;
                node = node.WithExplicitInterfaceSpecifier(explicitInterfaceSpecifier);

                var accessors = from a in node.AccessorList.Accessors
                                select a.WithExpressionBody(expression);
                node = node.WithAccessorList(SyntaxFactory.AccessorList(SyntaxFactory.List(accessors)));

                classSyntax = classSyntax.AddMembers(node);                
            }


            return classSyntax;
        }
 
    }


}