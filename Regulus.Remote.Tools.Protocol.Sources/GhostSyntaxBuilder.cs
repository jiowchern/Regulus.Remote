using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

namespace Regulus.Remote.Tools.Protocol.Sources
{
    public class GhostSyntaxBuilder
    {
        public readonly ClassDeclarationSyntax Ghost;
        public GhostSyntaxBuilder(InterfaceDeclarationSyntax interface_declaration)
        {

            var exc = SyntaxFactory.ArrowExpressionClause(SyntaxFactory.ThrowExpression(SyntaxFactory.ParseExpression("new SystemException(\"\")")));
            var explicitInterfaceSpecifier = SyntaxFactory.ExplicitInterfaceSpecifier(SyntaxFactory.ParseName(interface_declaration.Identifier.ToString()));

            var classSyntax = SyntaxFactory.ClassDeclaration($"C{interface_declaration.Identifier}");
            
            var nodes = from n in interface_declaration.DescendantNodes().OfType<MemberDeclarationSyntax>() select _Clone(n);

            var newNodes = new System.Collections.Generic.List<MemberDeclarationSyntax>();
            foreach (var node in nodes)
            {

                var newNode = node;
                if(node is EventFieldDeclarationSyntax eventFieldDeclarationSyntax)
                {                    
                    newNode = SyntaxFactory.EventDeclaration(eventFieldDeclarationSyntax.Declaration.Type, eventFieldDeclarationSyntax.Declaration.Variables[0].Identifier);
                }
                newNodes.Add(newNode);
            }

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
                node = node.WithAccessorList(SyntaxFactory.AccessorList(SyntaxFactory.List(new[] {
                 SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithExpressionBody(exc),
                 SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithExpressionBody(exc)})));


                classSyntax = classSyntax.AddMembers(node);
            }
            foreach (var member in newNodes.OfType<EventDeclarationSyntax>())
            {
                var node = member;
                node = node.WithExplicitInterfaceSpecifier(explicitInterfaceSpecifier);
                node = node.WithAccessorList(SyntaxFactory.AccessorList(SyntaxFactory.List(new[] {
                 SyntaxFactory.AccessorDeclaration(SyntaxKind.AddAccessorDeclaration).WithExpressionBody(exc),
                 SyntaxFactory.AccessorDeclaration(SyntaxKind.RemoveAccessorDeclaration).WithExpressionBody(exc)})));


                classSyntax = classSyntax.AddMembers(node);
            }

            foreach (var member in newNodes.OfType<IndexerDeclarationSyntax>())
            {
                var node = member;
                node = node.WithExplicitInterfaceSpecifier(explicitInterfaceSpecifier);
                node = node.WithAccessorList(SyntaxFactory.AccessorList(SyntaxFactory.List(new[] {
                 SyntaxFactory.AccessorDeclaration(SyntaxKind.AddAccessorDeclaration).WithExpressionBody(exc),
                 SyntaxFactory.AccessorDeclaration(SyntaxKind.RemoveAccessorDeclaration).WithExpressionBody(exc)})));


                classSyntax = classSyntax.AddMembers(node);
            }


            Ghost = classSyntax;
        }

        private MemberDeclarationSyntax _Clone(MemberDeclarationSyntax n)
        {

            using (var stream = new System.IO.MemoryStream())
            {
                n.SerializeTo(stream);
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                return CSharpSyntaxNode.DeserializeFrom(stream) as MemberDeclarationSyntax;
            }
                
        }
    }

}