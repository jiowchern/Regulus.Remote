using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
namespace Regulus.Remote.Tools.Protocol.Sources.Codes
{
    public static partial class RegulusRemoteIGhost
    {
        public static readonly MemberDeclarationSyntax Field_AddEventEvent = EventFieldDeclaration(
                        VariableDeclaration(
                            _RegulusRemoteEventNotifyCallback
                        )
                        .WithVariables(
                            SingletonSeparatedList(
                                VariableDeclarator(
                                    Identifier("_AddEventEvent")
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