using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Regulus.Remote.Tools.Protocol.Sources.Extensions;
using System.Linq;
namespace Regulus.Remote.Tools.Protocol.Sources.BlockModifiers
{

    internal class EventName
    {
        public FieldDeclarationSyntax Field;
        public static EventName Mod(System.Collections.Generic.IEnumerable<SyntaxNode> nodes)
        {
            return null;
        }

        private static FieldDeclarationSyntax _CreateGhostEventHandler(string name)
        {
            return SyntaxFactory.FieldDeclaration(
             SyntaxFactory.VariableDeclaration(
                 SyntaxFactory.QualifiedName(
                     SyntaxFactory.QualifiedName(
                         SyntaxFactory.IdentifierName("Regulus"),
                         SyntaxFactory.IdentifierName("Remote")),
                     SyntaxFactory.IdentifierName("GhostEventHandler")))
             .WithVariables(
                 SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                     SyntaxFactory.VariableDeclarator(
                         SyntaxFactory.Identifier(name))
                     .WithInitializer(
                         SyntaxFactory.EqualsValueClause(
                             SyntaxFactory.ObjectCreationExpression(
                                 SyntaxFactory.QualifiedName(
                                     SyntaxFactory.QualifiedName(
                                         SyntaxFactory.IdentifierName("Regulus"),
                                         SyntaxFactory.IdentifierName("Remote")),
                                     SyntaxFactory.IdentifierName("GhostEventHandler")))
                             .WithArgumentList(
                                 SyntaxFactory.ArgumentList()))))));
        }
    }
    internal class Event
    {
        public BlockSyntax Block;
        
        public System.Collections.Generic.IEnumerable<TypeSyntax> Types;

        public static Event Mod(System.Collections.Generic.IEnumerable<SyntaxNode> nodes)
        {
            var block = nodes.Skip(0).FirstOrDefault() as BlockSyntax;
            var ad = nodes.Skip(1).FirstOrDefault() as AccessorDeclarationSyntax;
            var al = nodes.Skip(2).FirstOrDefault() as AccessorListSyntax;
            var ed = nodes.Skip(3).FirstOrDefault() as EventDeclarationSyntax;
            var cd = nodes.Skip(4).FirstOrDefault() as ClassDeclarationSyntax;
            
            if (Extensions.SyntaxExtensions.AnyNull(block, ad, al, ed, cd))
            {
                return null; 
            }

            var ownerName = ed.ExplicitInterfaceSpecifier.Name;
            var name = $"_{ownerName}.{ed.Identifier}";
            name = name.Replace('.', '_');
            
            string ghostEventHandlerMethod = "";
            if (ad.IsKind(SyntaxKind.AddAccessorDeclaration))
            {
                ghostEventHandlerMethod = "Add";
            }
            else if (ad.IsKind(SyntaxKind.RemoveAccessorDeclaration))
            {
                ghostEventHandlerMethod = "Remove";
            }

            var newBlock = SyntaxFactory.Block(SyntaxFactory.ParseStatement(
$@"
var id = {name}.{ghostEventHandlerMethod}(value);
_{ghostEventHandlerMethod}EventEvent(typeof({ownerName}).GetEvent(""{ed.Identifier}""),id);
"));

            return new Event() { Block = newBlock, Types  = new TypeSyntax[0]};
        }


        

        
    }
}