using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
namespace Regulus.Remote.Tools.Protocol.Sources.Codes
{
    public static partial class RegulusRemoteIGhost
    {
        public static readonly MemberDeclarationSyntax Field_CallMethodEvent = EventFieldDeclaration(
                        VariableDeclaration(
                            _RegulusRemoteCallMethodCallback
                        )
                        .WithVariables(
                            SingletonSeparatedList(
                                VariableDeclarator(
                                    Identifier("_CallMethodEvent")
                                )
                            )
                        )
                    )
                    .WithModifiers(
                        TokenList(
                            Token(SyntaxKind.PrivateKeyword)
                        )
                    );
    }
}