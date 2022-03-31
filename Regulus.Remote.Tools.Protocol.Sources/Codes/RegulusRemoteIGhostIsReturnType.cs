using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
namespace Regulus.Remote.Tools.Protocol.Sources.Codes
{
    public static partial class RegulusRemoteIGhost
    {
        public static readonly MemberDeclarationSyntax IsReturnType = MethodDeclaration(
                PredefinedType(
                    Token(SyntaxKind.BoolKeyword)
                ),
                Identifier("IsReturnType")
            )
            .WithExplicitInterfaceSpecifier(
                ExplicitInterfaceSpecifier(
                    _RegulusRemoteIGhost
                )
            )
            .WithBody(
                Block(
                    SingletonList(
                        (StatementSyntax)ReturnStatement(
                            IdentifierName("_HaveReturn")
                        )
                    )
                )
            );

    }
}