using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Regulus.Remote.Tools.Protocol.Sources.Extensions;
namespace Regulus.Remote.Tools.Protocol.Sources
{

    class GhostBuilder  
    {
        
        public readonly IEnumerable<TypeSyntax> Types;
        public readonly IEnumerable<ClassDeclarationSyntax> Ghosts;
        public readonly IEnumerable<ClassDeclarationSyntax> EventProxys;
        public readonly IEnumerable<InterfaceDeclarationSyntax> Souls;
        public readonly string Namespace;
        public readonly IEnumerable<ClassAndTypes> ClassAndTypess;

        public GhostBuilder(SyntaxModifier modifier, IEnumerable<INamedTypeSymbol> symbols)
        {
            
            var builders = new Dictionary<INamedTypeSymbol, InterfaceInheritor>(SymbolEqualityComparer.Default);

            var souls = new List<InterfaceDeclarationSyntax>();
            foreach (var item in from i in symbols
                                 select new KeyValuePair<INamedTypeSymbol, InterfaceInheritor>(i, new InterfaceInheritor(i.ToInferredInterface())))
            {
                if(!builders.ContainsKey(item.Key))
                {
                    builders.Add(item.Key, item.Value);
                    souls.Add(item.Value.Base);
                }                    
            }
            Souls = souls;

            var types  = new System.Collections.Generic.List<TypeSyntax>();
            var ghosts = new System.Collections.Generic.List<ClassDeclarationSyntax>();
            var eventProxys= new System.Collections.Generic.List<ClassDeclarationSyntax>();
            
            var classAndTypess = new System.Collections.Generic.List<ClassAndTypes>();
            foreach (var symbol in symbols)
            {
              
                var name = $"C{symbol.ToDisplayString().Replace('.', '_')}";
                var type = SyntaxFactory.ClassDeclaration(name);
                type = type.WithOpenBraceToken(
                     SyntaxFactory.Token(
                        SyntaxFactory.TriviaList(
                            SyntaxFactory.Trivia(
                                SyntaxFactory.PragmaWarningDirectiveTrivia(
                                    SyntaxFactory.Token(SyntaxKind.DisableKeyword),
                                    true
                                )
                                .WithErrorCodes(
                                    SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(
                                        SyntaxFactory.IdentifierName("CS0067")
                                    )
                                )
                            )
                        ),
                        SyntaxKind.OpenBraceToken,
                        SyntaxFactory.TriviaList()
                    )
                );
                foreach (var i2 in symbol.AllInterfaces.Union(new[] { symbol }))
                {
                    var builder = builders[i2];
                    type = builder.Inherite(type);
                }

                eventProxys.AddRange(type.DescendantNodes().OfType<EventDeclarationSyntax>().Select(e => e.CreateRegulusRemoteIEventProxyCreater()));

                var classAndTypes = modifier.Mod(type);
                classAndTypess.Add(classAndTypes);
                types.AddRange(classAndTypes.TypesOfSerialization);
                type = classAndTypes.Type; 
                type = type.ImplementRegulusRemoteIGhost();
                type = type.WithCloseBraceToken(
                            SyntaxFactory.Token(
                                SyntaxFactory.TriviaList(
                                    SyntaxFactory.Trivia(
                                        SyntaxFactory.PragmaWarningDirectiveTrivia(
                                            SyntaxFactory.Token(SyntaxKind.RestoreKeyword),
                                            true
                                        )
                                        .WithErrorCodes(
                                            SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(
                                                SyntaxFactory.IdentifierName("CS0067")
                                            )
                                        )
                                    )
                                ),
                                SyntaxKind.CloseBraceToken,
                                SyntaxFactory.TriviaList()
                            )
                        );
                ghosts.Add(type);

            }
            ClassAndTypess = classAndTypess;
            Types = new HashSet<TypeSyntax>(_WithOutNamespaceFilter(types) , SyntaxNodeComparer.Default);
            Ghosts = new HashSet<ClassDeclarationSyntax>(ghosts, SyntaxNodeComparer.Default);
            EventProxys = new HashSet<ClassDeclarationSyntax>(eventProxys, SyntaxNodeComparer.Default);


            var all = string.Join("", Types.Select(t => t.ToFullString()).Union(Ghosts.Select(g => g.Identifier.ToFullString())).Union(EventProxys.Select(e => e.Identifier.ToFullString())));


            Namespace = $"RegulusRemoteProtocol{all.ToMd5().ToMd5String().Replace("-","")}";


        }

        private IEnumerable<TypeSyntax> _WithOutNamespaceFilter(List<TypeSyntax> types)
        {
            foreach (var type in types)
            {
                yield return type;
            }
        }
    }

}