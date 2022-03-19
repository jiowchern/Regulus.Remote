using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using System.Linq;

namespace Regulus.Remote.Tools.Protocol.Sources.BlockModifiers
{
    internal class MethodVoid
    {
        public BlockSyntax Block;
        public System.Collections.Generic.IEnumerable<TypeSyntax> Types;
        public static MethodVoid Mod(System.Collections.Generic.IEnumerable<SyntaxNode> nodes)
        {
            var block = nodes.Skip(0).FirstOrDefault() as BlockSyntax;            
            var md = nodes.Skip(1).FirstOrDefault() as MethodDeclarationSyntax;
            var cd = nodes.Skip(2).FirstOrDefault() as ClassDeclarationSyntax;

            if (Extensions.SyntaxExtensions.AnyNull(block,  md, cd))
            {
                return null;
            }

            if((from p in md.ParameterList.Parameters
            from m in p.Modifiers
            where m.IsKind(SyntaxKind.OutKeyword)
            select m).Any())
                return null;

            var interfaceCode = md.ExplicitInterfaceSpecifier.Name;
            var methodCode = md.Identifier.ToFullString();
            var methodCallParamsCode = string.Join(",", from p in md.ParameterList.Parameters select p.Identifier.ToFullString());
            var returnType = md.ReturnType;
            var pt = returnType as PredefinedTypeSyntax;

            if(pt == null)
                return null;

            if (!pt.Keyword.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.VoidKeyword))
            {
                return null;
            }

            var b = _Build();


            return new MethodVoid
            { 
                Block = SyntaxFactory.Block(SyntaxFactory.ParseStatement(
$@"var info = typeof({interfaceCode}).GetMethod(""{methodCode}"");
this._CallMethodEvent(info , new object[] {{{methodCallParamsCode}}} , null);",0)),
                Types = from p in md.ParameterList.Parameters select p.Type
                };
            
            
        }


        static BlockSyntax _Build()
        {
            return Block(
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
                                                    Identifier("info")
                                                )
                                                .WithInitializer(
                                                    EqualsValueClause(
                                                        InvocationExpression(
                                                            MemberAccessExpression(
                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                TypeOfExpression(
                                                                    QualifiedName(
                                                                        IdentifierName("NS1"),
                                                                        IdentifierName("IA")
                                                                    )
                                                                ),
                                                                IdentifierName("GetMethod")
                                                            )
                                                        )
                                                        .WithArgumentList(
                                                            ArgumentList(
                                                                SingletonSeparatedList<ArgumentSyntax>(
                                                                    Argument(
                                                                        LiteralExpression(
                                                                            SyntaxKind.StringLiteralExpression,
                                                                            Literal("M123")
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
                                    ExpressionStatement(
                                        InvocationExpression(
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                ThisExpression(),
                                                IdentifierName("_CallMethodEvent")
                                            )
                                        )
                                        .WithArgumentList(
                                            ArgumentList(
                                                SeparatedList<ArgumentSyntax>(
                                                    new SyntaxNodeOrToken[]{
                                                        Argument(
                                                            IdentifierName("info")
                                                        ),
                                                        Token(SyntaxKind.CommaToken),
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
                                                                    SingletonSeparatedList<ExpressionSyntax>(
                                                                        IdentifierName("_1")
                                                                    )
                                                                )
                                                            )
                                                        ),
                                                        Token(SyntaxKind.CommaToken),
                                                        Argument(
                                                            LiteralExpression(
                                                                SyntaxKind.NullLiteralExpression
                                                            )
                                                        )
                                                    }
                                                )
                                            )
                                        )
                                    )
                                );
        }
    }
}