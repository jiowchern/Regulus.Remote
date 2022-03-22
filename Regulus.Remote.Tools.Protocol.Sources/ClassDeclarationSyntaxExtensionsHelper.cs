using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
namespace Regulus.Remote.Tools.Protocol.Sources
{
    public static class ClassDeclarationSyntaxExtensionsHelper
    {
        public static BlockSyntax _Block(SyntaxKind exp_1, string field_name)
        {
            return Block(
                        SingletonList<StatementSyntax>(
                                ExpressionStatement(
                                    AssignmentExpression(
                                        exp_1,
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            ThisExpression(),
                                            IdentifierName(field_name)
                                        ),
                                        IdentifierName("value")
                                    )
                                )
                            )
                        );
        }

        public static readonly QualifiedNameSyntax _RegulusRemoteEventNotifyCallback = QualifiedName(
                                QualifiedName(
                                    IdentifierName("Regulus"),
                                    IdentifierName("Remote")
                                ),
                                IdentifierName("EventNotifyCallback")
                            );
        public static readonly QualifiedNameSyntax _RegulusRemoteCallMethodCallback = QualifiedName(
                            QualifiedName(
                                IdentifierName("Regulus"),
                                IdentifierName("Remote")
                            ),
                            IdentifierName("CallMethodCallback")
                        );
        public static readonly QualifiedNameSyntax _RegulusRemoteIGhost = QualifiedName
                        (
                            QualifiedName(
                                IdentifierName("Regulus"),
                                IdentifierName("Remote")
                            )
                            .WithDotToken(
                                Token(SyntaxKind.DotToken)
                            ),
                            IdentifierName("IGhost")
                        );
    }
}