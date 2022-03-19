using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
namespace Regulus.Remote.Tools.Protocol.Sources.Modifiers
{
    internal class PropertyFieldDeclarationSyntax
    {
        public FieldDeclarationSyntax Field;
        public System.Collections.Generic.IEnumerable<TypeSyntax> Types;

        public static PropertyFieldDeclarationSyntax Mod(PropertyDeclarationSyntax pd)
        {

            var ownerName = pd.ExplicitInterfaceSpecifier.Name;
            var name = $"_{ownerName}.{pd.Identifier}";
            name = name.Replace('.', '_');

            var types = new System.Collections.Generic.List<TypeSyntax>();
            var qn = pd.Type as QualifiedNameSyntax;

            if (qn == null)
                return null;

            if (qn.Left.ToString() != "Regulus.Remote")
                return null;

            var sn = qn.Right ;
            if (sn == null)
                return null;

            if (sn.Identifier.ToString() != "Property" && sn.Identifier.ToString() != "Notifier")
                return null;

            if(!pd.DescendantNodes().OfType<AccessorDeclarationSyntax>().Any( a=>a.Kind() == SyntaxKind.GetAccessorDeclaration))
                return null;

            types.AddRange(qn.Right.DescendantNodes().OfType<TypeSyntax>());

            return new PropertyFieldDeclarationSyntax() { Field = _CreateField(name, qn), Types = types };
        }

        private static FieldDeclarationSyntax _CreateField(string name, QualifiedNameSyntax qn)
        {
            return SyntaxFactory.FieldDeclaration(
                    SyntaxFactory.VariableDeclaration(
                        qn
                    )
                    .WithVariables(
                        SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                            SyntaxFactory.VariableDeclarator(
                                SyntaxFactory.Identifier(name)
                            )
                            .WithInitializer(
                                SyntaxFactory.EqualsValueClause(
                                    SyntaxFactory.ObjectCreationExpression(
                                        qn
                                    )
                                    .WithArgumentList(
                                       SyntaxFactory.ArgumentList()
                                    )
                                )
                            )
                        )
                    )
                );
        }
    }
}
