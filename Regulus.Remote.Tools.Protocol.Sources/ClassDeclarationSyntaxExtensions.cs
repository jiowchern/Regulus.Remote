using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
namespace Regulus.Remote.Tools.Protocol.Sources
{
    public static class ClassDeclarationSyntaxExtensions
    {
        static BlockSyntax _Block(SyntaxKind exp_1 ,string field_name)
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

        static readonly QualifiedNameSyntax _RegulusRemoteEventNotifyCallback = QualifiedName(
                                QualifiedName(
                                    IdentifierName("Regulus"),
                                    IdentifierName("Remote")
                                ),
                                IdentifierName("EventNotifyCallback")
                            );
        static readonly QualifiedNameSyntax _RegulusRemoteCallMethodCallback = QualifiedName(
                            QualifiedName(
                                IdentifierName("Regulus"),
                                IdentifierName("Remote")
                            ),
                            IdentifierName("CallMethodCallback")
                        );
        static readonly QualifiedNameSyntax _RegulusRemoteIGhost = QualifiedName
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

        public static ClassDeclarationSyntax ImplementRegulusRemoteIGhost(this ClassDeclarationSyntax class_declaration)
        {
            var cd = class_declaration;            

         
            var type = SimpleBaseType(_RegulusRemoteIGhost);

            var baseList = cd.BaseList ?? SyntaxFactory.BaseList();

            var baseTypes = baseList.Types.ToArray();
            
            baseList = baseList.WithTypes(new SeparatedSyntaxList<BaseTypeSyntax>().Add(type).AddRange(baseTypes));
            cd = cd.WithBaseList(baseList);

            cd = cd.AddMembers(_CreateMembers(class_declaration.Identifier));
            

            return cd;
        }

        public static MemberDeclarationSyntax[] _CreateMembers(SyntaxToken name)
        {
            return new MemberDeclarationSyntax[]{
                    FieldDeclaration(
                        VariableDeclaration(
                            PredefinedType(
                                Token(SyntaxKind.BoolKeyword)
                            )
                        )
                        .WithVariables(
                            SingletonSeparatedList<VariableDeclaratorSyntax>(
                                VariableDeclarator(
                                    Identifier("_HaveReturn")
                                )
                            )
                        )
                    )
                    .WithModifiers(
                        TokenList(
                            Token(SyntaxKind.ReadOnlyKeyword)
                        )
                    ),
                    FieldDeclaration(
                        VariableDeclaration(
                            PredefinedType(
                                Token(SyntaxKind.LongKeyword)
                            )
                        )
                        .WithVariables(
                            SingletonSeparatedList<VariableDeclaratorSyntax>(
                                VariableDeclarator(
                                    Identifier("_GhostId")
                                )
                            )
                        )
                    )
                    .WithModifiers(
                        TokenList(
                            Token(SyntaxKind.ReadOnlyKeyword)
                        )
                    ),
                    ConstructorDeclaration(
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
                    ),
                    MethodDeclaration(
                        PredefinedType(
                            Token(SyntaxKind.LongKeyword)
                        ),
                        Identifier("GetID")
                    )
                    .WithExplicitInterfaceSpecifier(
                        ExplicitInterfaceSpecifier(
                            _RegulusRemoteIGhost
                        )
                    )
                    .WithBody(
                        Block(
                            SingletonList<StatementSyntax>(
                                ReturnStatement(
                                    IdentifierName("_GhostId")
                                )
                            )
                        )
                    ),
                    MethodDeclaration(
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
                            SingletonList<StatementSyntax>(
                                ReturnStatement(
                                    IdentifierName("_HaveReturn")
                                )
                            )
                        )
                    ),
                    MethodDeclaration(
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
                            SingletonList<StatementSyntax>(
                                ReturnStatement(
                                    ThisExpression()
                                )
                            )
                        )
                    ),
                    EventFieldDeclaration(
                        VariableDeclaration(
                            _RegulusRemoteCallMethodCallback
                        )
                        .WithVariables(
                            SingletonSeparatedList<VariableDeclaratorSyntax>(
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
                    ),
                    EventDeclaration(
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
                            List<AccessorDeclarationSyntax>(
                                new AccessorDeclarationSyntax[]{
                                    AccessorDeclaration(
                                        SyntaxKind.AddAccessorDeclaration
                                    )
                                    .WithBody(
                                        _Block(SyntaxKind.AddAssignmentExpression ,"_CallMethodEvent")
                                    ),
                                    AccessorDeclaration(
                                        SyntaxKind.RemoveAccessorDeclaration
                                    )
                                    .WithBody(
                                        _Block(SyntaxKind.SubtractAssignmentExpression ,"_CallMethodEvent")
                                    )
                                }
                            )
                        )
                    ),
                    EventFieldDeclaration(
                        VariableDeclaration(
                            _RegulusRemoteEventNotifyCallback
                        )
                        .WithVariables(
                            SingletonSeparatedList<VariableDeclaratorSyntax>(
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
                    ),
                    EventDeclaration(
                        _RegulusRemoteEventNotifyCallback,
                        Identifier("AddEventEvent")
                    )
                    .WithExplicitInterfaceSpecifier(
                        ExplicitInterfaceSpecifier(
                            _RegulusRemoteIGhost
                        )
                    )
                    .WithAccessorList(
                        AccessorList(
                            List<AccessorDeclarationSyntax>(
                                new AccessorDeclarationSyntax[]{
                                    AccessorDeclaration(
                                        SyntaxKind.AddAccessorDeclaration
                                    )
                                    .WithBody(
                                        _Block(SyntaxKind.AddAssignmentExpression , "_AddEventEvent")                                        
                                    ),
                                    AccessorDeclaration(
                                        SyntaxKind.RemoveAccessorDeclaration
                                    )
                                    .WithBody(
                                        _Block(SyntaxKind.SubtractAssignmentExpression, "_AddEventEvent")                                        
                                    )
                                }
                            )
                        )
                    ),
                    EventFieldDeclaration(
                        VariableDeclaration(
                            _RegulusRemoteEventNotifyCallback
                        )
                        .WithVariables(
                            SingletonSeparatedList<VariableDeclaratorSyntax>(
                                VariableDeclarator(
                                    Identifier("_RemoveEventEvent")
                                )
                            )
                        )
                    )
                    .WithModifiers(
                        TokenList(
                            Token(SyntaxKind.PrivateKeyword)
                        )
                    ),
                    EventDeclaration(
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
                            List<AccessorDeclarationSyntax>(
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
                    )
                };
        }

    }
}