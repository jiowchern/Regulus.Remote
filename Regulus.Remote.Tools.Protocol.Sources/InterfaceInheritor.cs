using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Regulus.Remote.Tools.Protocol.Sources.Extensions;

using System.Linq;

namespace Regulus.Remote.Tools.Protocol.Sources
{
    public class InterfaceInheritor
    {
        readonly string _NamePath;
        public readonly InterfaceDeclarationSyntax Base;
        
        public readonly BlockSyntax Expression;
        

        public InterfaceInheritor(InterfaceDeclarationSyntax @base) :this(@base , SyntaxFactory.Block(SyntaxFactory.ExpressionStatement(SyntaxFactory.ThrowExpression(SyntaxFactory.ParseExpression("new Regulus.Remote.Exceptions.NotSupportedException()")))))
        {
            
        }
        public InterfaceInheritor(InterfaceDeclarationSyntax @base, BlockSyntax expression)
        {
            Expression = expression;
            _NamePath = @base.GetNamePath();
            Expression = expression;
            this.Base = @base;
        }

        public ClassDeclarationSyntax Inherite(ClassDeclarationSyntax class_syntax)
        {
            
            var interfaceDeclaration = Base;
            var namePath = _NamePath;

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
                node = node.WithBody(expression);
                classSyntax = classSyntax.AddMembers(node);
            }

            foreach (var member in nodes.OfType<PropertyDeclarationSyntax>())
            {
                var node = member;
                node = node.WithExplicitInterfaceSpecifier(explicitInterfaceSpecifier);

                var accessors = from a in node.AccessorList.Accessors
                                select a.WithBody(expression);

                node = node.WithAccessorList(SyntaxFactory.AccessorList(SyntaxFactory.List(accessors)));

                classSyntax = classSyntax.AddMembers(node);

            }

            foreach (var member in nodes.OfType<EventFieldDeclarationSyntax>())
            {
                var node = member;
                var eventDeclaration = SyntaxFactory.EventDeclaration(node.Declaration.Type, node.Declaration.Variables[0].Identifier);
                
                eventDeclaration = eventDeclaration.WithExplicitInterfaceSpecifier(explicitInterfaceSpecifier);
                var accessors = SyntaxFactory.AccessorList();
                accessors = accessors.AddAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.AddAccessorDeclaration).WithBody(expression));
                accessors = accessors.AddAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.RemoveAccessorDeclaration).WithBody(expression));
                eventDeclaration = eventDeclaration.WithAccessorList(accessors);
                classSyntax = classSyntax.AddMembers(eventDeclaration);
            }
           

            foreach (var member in nodes.OfType<IndexerDeclarationSyntax>())
            {
                var node = member;
                node = node.WithExplicitInterfaceSpecifier(explicitInterfaceSpecifier);

                var accessors = from a in node.AccessorList.Accessors
                                select a.WithBody(expression);
                node = node.WithAccessorList(SyntaxFactory.AccessorList(SyntaxFactory.List(accessors)));

                classSyntax = classSyntax.AddMembers(node);                
            }


            return classSyntax;
        }
 
    }


}