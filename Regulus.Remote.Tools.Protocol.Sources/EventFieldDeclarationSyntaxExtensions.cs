using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
namespace Regulus.Remote.Tools.Protocol.Sources
{
    
    public static class EventFieldDeclarationSyntaxExtensions
    {
        public static ClassDeclarationSyntax CreateRegulusRemoteIEventProxyCreater(this EventDeclarationSyntax eds)
        {

            var een = eds.ExplicitInterfaceSpecifier.Name.ToString().Replace('.','_');

            var fieldName = eds.Identifier.ValueText;
            var typeName = eds.ExplicitInterfaceSpecifier.Name.ToString();
            var className = $"C{een}_{eds.Identifier}";
            var baseName = QualifiedName(
                            QualifiedName(
                                IdentifierName("Regulus"),
                                IdentifierName("Remote")
                            ),
                            IdentifierName("IEventProxyCreater")
                        );

            var paramList = SyntaxFactory.ParameterList();


            var paramExpression = "";
            if (eds.Type is QualifiedNameSyntax qn)
            {
                
                var typeList = qn.Right.DescendantNodes().OfType<TypeArgumentListSyntax>().FirstOrDefault() ?? SyntaxFactory.TypeArgumentList();
                var paramCount = typeList.Arguments.Count;

                var names = new string[paramCount];
                for (int i = 0; i < paramCount; i++)
                {
                    var name = $"_{i + 1 }";
                    names[i] = name;
                    paramList = paramList.AddParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier(name)));
                }
                paramExpression = string.Join(",", names);
            }
            var sl = SyntaxFactory.SeparatedList<ExpressionSyntax>(new SyntaxNodeOrToken[] { SyntaxFactory.ParseExpression(paramExpression) });





            return ClassDeclaration(className)
        .WithBaseList(
            BaseList(
                SingletonSeparatedList<BaseTypeSyntax>(
                    SimpleBaseType(
                        baseName
                    )
                )
            )
        )
        .WithMembers(
            List<MemberDeclarationSyntax>(
                new MemberDeclarationSyntax[]{
                    FieldDeclaration(
                        VariableDeclaration(
                            QualifiedName(
                                IdentifierName("System"),
                                IdentifierName("Type")
                            )
                        )
                        .WithVariables(
                            SingletonSeparatedList<VariableDeclaratorSyntax>(
                                VariableDeclarator(
                                    Identifier("_Type")
                                )
                            )
                        )
                    ),
                    FieldDeclaration(
                        VariableDeclaration(
                            PredefinedType(
                                Token(SyntaxKind.StringKeyword)
                            )
                        )
                        .WithVariables(
                            SingletonSeparatedList<VariableDeclaratorSyntax>(
                                VariableDeclarator(
                                    Identifier("_Name")
                                )
                            )
                        )
                    ),
                    ConstructorDeclaration(
                        Identifier(className)
                    )
                    .WithModifiers(
                        TokenList(
                            Token(SyntaxKind.PublicKeyword)
                        )
                    )
                    .WithBody(
                        Block(
                            ExpressionStatement(
                                AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    IdentifierName("_Name"),
                                    LiteralExpression(
                                        SyntaxKind.StringLiteralExpression,
                                        Literal(fieldName)
                                    )
                                )
                            ),
                            ExpressionStatement(
                                AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    IdentifierName("_Type"),
                                    TypeOfExpression(
                                        IdentifierName(typeName)
                                    )
                                )
                            )
                        )
                    ),
                    MethodDeclaration(
                        QualifiedName(
                            IdentifierName("System"),
                            IdentifierName("Delegate")
                        ),
                        Identifier("Create")
                    )
                    .WithExplicitInterfaceSpecifier(
                        ExplicitInterfaceSpecifier(
                            baseName
                        )
                    )
                    .WithParameterList(
                        ParameterList(
                            SeparatedList<ParameterSyntax>(
                                new SyntaxNodeOrToken[]{
                                    Parameter(
                                        Identifier("soul_id")
                                    )
                                    .WithType(
                                        PredefinedType(
                                            Token(SyntaxKind.LongKeyword)
                                        )
                                    ),
                                    Token(SyntaxKind.CommaToken),
                                    Parameter(
                                        Identifier("event_id")
                                    )
                                    .WithType(
                                        PredefinedType(
                                            Token(SyntaxKind.IntKeyword)
                                        )
                                    ),
                                    Token(SyntaxKind.CommaToken),
                                    Parameter(
                                        Identifier("handler_id")
                                    )
                                    .WithType(
                                        PredefinedType(
                                            Token(SyntaxKind.LongKeyword)
                                        )
                                    ),
                                    Token(SyntaxKind.CommaToken),
                                    Parameter(
                                        Identifier("invoke_Event")
                                    )
                                    .WithType(
                                        baseName
                                    )
                                }
                            )
                        )
                    )
                    .WithBody(
                        Block(
                            LocalDeclarationStatement(
                                VariableDeclaration(
                                    IdentifierName(
                                        Identifier(
                                            TriviaList(),
                                            SyntaxKind.VarKeyword,
                                            "var",
                                            "var",
                                            TriviaList()
                                        )
                                    )
                                )
                                .WithVariables(
                                    SingletonSeparatedList<VariableDeclaratorSyntax>(
                                        VariableDeclarator(
                                            Identifier("closure")
                                        )
                                        .WithInitializer(
                                            EqualsValueClause(
                                                ObjectCreationExpression(
                                                    QualifiedName(
                                                        QualifiedName(
                                                            IdentifierName("Regulus"),
                                                            IdentifierName("Remote")
                                                        ),
                                                        IdentifierName("GenericEventClosure")
                                                    )
                                                )
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SeparatedList<ArgumentSyntax>(
                                                            new SyntaxNodeOrToken[]{
                                                                Argument(
                                                                    IdentifierName("soul_id")
                                                                ),
                                                                Token(SyntaxKind.CommaToken),
                                                                Argument(
                                                                    IdentifierName("event_id")
                                                                ),
                                                                Token(SyntaxKind.CommaToken),
                                                                Argument(
                                                                    IdentifierName("handler_id")
                                                                ),
                                                                Token(SyntaxKind.CommaToken),
                                                                Argument(
                                                                    IdentifierName("invoke_Event")
                                                                )
                                                            }
                                                        )
                                                    )
                                                )
                                            )
                                        )
                                    )
                                )
                            ),
                            ReturnStatement(
                                ObjectCreationExpression(
                                    eds.Type
                                )
                                .WithArgumentList(
                                    ArgumentList(
                                        SingletonSeparatedList<ArgumentSyntax>(
                                            Argument(
                                                ParenthesizedLambdaExpression()
                                                .WithParameterList(
                                                    paramList
                                                )
                                                .WithExpressionBody(
                                                    InvocationExpression(
                                                        MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            IdentifierName("closure"),
                                                            IdentifierName("Run")
                                                        )
                                                    )
                                                    .WithArgumentList(
                                                        ArgumentList(
                                                            SingletonSeparatedList<ArgumentSyntax>(
                                                                Argument(
                                                                    ArrayCreationExpression(
                                                                        ArrayType(
                                                                            PredefinedType(
                                                                                Token(SyntaxKind.ObjectKeyword)
                                                                            )
                                                                        )
                                                                        .WithRankSpecifiers(
                                                                            SingletonList<ArrayRankSpecifierSyntax>(
                                                                                ArrayRankSpecifier(
                                                                                    SingletonSeparatedList<ExpressionSyntax>(
                                                                                        OmittedArraySizeExpression()
                                                                                    )
                                                                                )
                                                                            )
                                                                        )
                                                                    )
                                                                    .WithInitializer(
                                                                        InitializerExpression(
                                                                            SyntaxKind.ArrayInitializerExpression,
                                                                            SeparatedList<ExpressionSyntax>(
                                                                                sl
                                                                            )
                                                                        )
                                                                    )
                                                                )
                                                            )
                                                        )
                                                    )
                                                )
                                            )
                                        )
                                    )
                                )
                            )
                        )
                    ),
                    MethodDeclaration(
                        QualifiedName(
                            IdentifierName("System"),
                            IdentifierName("Type")
                        ),
                        Identifier("GetType")
                    )
                    .WithExplicitInterfaceSpecifier(
                        ExplicitInterfaceSpecifier(
                            baseName
                        )
                    )
                    .WithBody(
                        Block(
                            SingletonList<StatementSyntax>(
                                ReturnStatement(
                                    IdentifierName("_Type")
                                )
                            )
                        )
                    ),
                    MethodDeclaration(
                        PredefinedType(
                            Token(SyntaxKind.StringKeyword)
                        ),
                        Identifier("GetName")
                    )
                    .WithExplicitInterfaceSpecifier(
                        ExplicitInterfaceSpecifier(
                            baseName
                        )
                    )
                    .WithBody(
                        Block(
                            SingletonList<StatementSyntax>(
                                ReturnStatement(
                                    IdentifierName("_Name")
                                )
                            )
                        )
                    )
                }
            )
        );
        }
    }
}