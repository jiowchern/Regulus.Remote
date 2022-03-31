using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
namespace Regulus.Remote.Tools.Protocol.Sources.Codes
{
    public static partial class RegulusRemoteIGhost
    {
        public static ConstructorDeclarationSyntax Constructor(SyntaxToken name)
        {
            return ConstructorDeclaration(
                                    name
                                )
                                .WithModifiers(
                                    TokenList(
                                        Token(SyntaxKind.PublicKeyword)
                                    )
                                )
                                .WithParameterList(
                                    ParameterList(
                                        SeparatedList<ParameterSyntax>(
                                            new SyntaxNodeOrToken[]{
                                    Parameter(
                                        Identifier("id")
                                    )
                                    .WithType(
                                        PredefinedType(
                                            Token(SyntaxKind.LongKeyword)
                                        )
                                    ),
                                    Token(SyntaxKind.CommaToken),
                                    Parameter(
                                        Identifier("have_return")
                                    )
                                    .WithType(
                                        PredefinedType(
                                            Token(SyntaxKind.BoolKeyword)
                                        )
                                    )
                                            }
                                        )
                                    )
                                )
                                .WithBody(
                                    Block(
                                        ExpressionStatement(
                                            AssignmentExpression(
                                                SyntaxKind.SimpleAssignmentExpression,
                                                IdentifierName("_GhostId"),
                                                IdentifierName("id")
                                            )
                                        ),
                                        ExpressionStatement(
                                            AssignmentExpression(
                                                SyntaxKind.SimpleAssignmentExpression,
                                                IdentifierName("_HaveReturn"),
                                                IdentifierName("have_return")
                                            )
                                        )
                                    )
                                );
        }
    }
}