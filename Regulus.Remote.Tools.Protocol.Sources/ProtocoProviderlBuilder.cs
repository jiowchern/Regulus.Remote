using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Text;

namespace Regulus.Remote.Tools.Protocol.Sources
{
    internal class ProtocoProviderlBuilder
    {

        public readonly SyntaxTree[] Trees;        

        public ProtocoProviderlBuilder(Compilation compilation,string protocol_name)
        {

            var trees = new System.Collections.Generic.List<SyntaxTree>();
            foreach (var tree in compilation.SyntaxTrees)
            {
                var root = tree.GetRoot();
                var model = compilation.GetSemanticModel(tree);
                var methods = root.DescendantNodes().OfType<MethodDeclarationSyntax>();
                foreach (var method in methods)
                {
                    if (!method.Modifiers.Any(m => m.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PartialKeyword)))
                        continue;

                    foreach (var attribute in method.AttributeLists.SelectMany(a=>a.Attributes))
                    {
                        var info = model.GetSymbolInfo(attribute);
                        
                        var name = info.Symbol.ContainingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                        if (name != "global::Regulus.Remote.Protocol.CreaterAttribute")
                        {
                            continue;
                        }

                        var firstParam = method.ParameterList.Parameters.FirstOrDefault();
                        if (firstParam == null)
                            continue;
                        
                        var stat =  SyntaxFactory.ParseStatement($"{firstParam.Identifier} = new {protocol_name}();");
                        var block = SyntaxFactory.Block(stat);

                        var newMethod = SyntaxFactory.MethodDeclaration(method.ReturnType, method.Identifier);
                        newMethod = newMethod.WithParameterList(method.ParameterList);
                        newMethod = newMethod.WithModifiers(method.Modifiers);
                        newMethod = newMethod.WithBody(block);

                        var parent = method.Parent as ClassDeclarationSyntax;
                        var newClass = SyntaxFactory.ClassDeclaration(parent.Identifier);
                        newClass = newClass.WithModifiers(parent.Modifiers);
                        newClass = newClass.WithMembers(new SyntaxList<MemberDeclarationSyntax>(new[] { newMethod }));


                        
                        var @namespace = parent.Ancestors().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();

                        
                        var compilationUnit = parent.Ancestors().OfType<CompilationUnitSyntax>().FirstOrDefault();
                        var syntaxFactory = SyntaxFactory.CompilationUnit();
                        syntaxFactory = syntaxFactory.WithUsings(compilationUnit.Usings);
                        if (@namespace != null)
                        {
                            var newNamespace = SyntaxFactory.NamespaceDeclaration(@namespace.Name);
                            newNamespace = newNamespace.AddMembers(newClass);


                            syntaxFactory = syntaxFactory.AddMembers(newNamespace);
                        }
                        else
                        {
                            syntaxFactory = syntaxFactory.AddMembers(newClass);
                        }
                        var guid = System.Guid.NewGuid();
                        var t = SyntaxFactory.ParseSyntaxTree(syntaxFactory.NormalizeWhitespace().ToFullString(), null, $"RegulusRemoteProtocol.{guid}.cs", Encoding.UTF8);
                        trees.Add(t);
                    }
                }
            }
            Trees = trees.ToArray();
        }
    }
}
