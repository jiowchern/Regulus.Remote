using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
namespace Regulus.Remote.Tools.Protocol.Sources.Codes
{
    public static partial class RegulusRemoteIGhost
    {
        public static readonly MemberDeclarationSyntax GetInstance = MethodDeclaration(
                        PredefinedType(
                            Token(SyntaxKind.ObjectKeyword)
                        ),
                        Identifier("GetInstance")
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
                                    ThisExpression()
                                )
                            )
                        )
                    );
    }
}