using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
namespace Regulus.Remote.Tools.Protocol.Sources.Codes
{
    public static partial class RegulusRemoteIGhost
    {
        public static readonly MemberDeclarationSyntax EventCallMethodEvent = EventDeclaration(
                           _RegulusRemoteCallMethodCallback

                       ,
                       Identifier("CallMethodEvent")
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
                                        _Block(SyntaxKind.AddAssignmentExpression,"_CallMethodEvent")
                                    ),
                                    AccessorDeclaration(
                                        SyntaxKind.RemoveAccessorDeclaration
                                    )
                                    .WithBody(
                                        _Block(SyntaxKind.SubtractAssignmentExpression,"_CallMethodEvent")
                                    )
                                }
                            )
                        )
                    );
    }
}