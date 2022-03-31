using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
namespace Regulus.Remote.Tools.Protocol.Sources.Codes
{
    public static partial class RegulusRemoteIGhost
    {
        public static readonly MemberDeclarationSyntax EventRemoveEventEvent = EventDeclaration(
                        _RegulusRemoteEventNotifyCallback,
                        Identifier("RemoveEventEvent")
                    )
                    .WithExplicitInterfaceSpecifier(
                        ExplicitInterfaceSpecifier(
                            _RegulusRemoteIGhost
                        )
                    )
                    .WithAccessorList(
                        AccessorList(
                            List(
                                new AccessorDeclarationSyntax[]{
                                    AccessorDeclaration(
                                        SyntaxKind.AddAccessorDeclaration
                                    )
                                    .WithBody(
                                        _Block(SyntaxKind.AddAssignmentExpression, "_RemoveEventEvent")
                                    ),
                                    AccessorDeclaration(
                                        SyntaxKind.RemoveAccessorDeclaration
                                    )
                                    .WithBody(
                                        _Block(SyntaxKind.SubtractAssignmentExpression, "_RemoveEventEvent")
                                    )
                                }
                            )
                        )
                    );
    }
}