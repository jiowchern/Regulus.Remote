using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using static Microsoft.CodeAnalysis.SyntaxNodeExtensions;
namespace Regulus.Remote.Tools.Protocol.Sources
{
    public class SyntaxReplacer
    {
        
        public readonly System.Collections.Generic.IEnumerable<TypeSyntax>  TypesOfSerialization;
        public readonly ClassDeclarationSyntax Type;
        public SyntaxReplacer(ClassDeclarationSyntax type)
        {
            

            var blocks = type.DescendantNodes().OfType<BlockSyntax>().ToArray();

            var typesOfSerialization = new System.Collections.Generic.List<TypeSyntax>();
            foreach (var block in blocks)
            {
                if(block.Parent is MethodDeclarationSyntax method)
                {
                    var returnType = method.ReturnType;
                    if(returnType is PredefinedTypeSyntax pt)
                    {
                        if(pt.Keyword.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.VoidKeyword))
                        {
                            var interfaceCode = method.ExplicitInterfaceSpecifier.ToFullString();
                            var methodCode = method.Identifier.ToFullString();
                            var methodCallParamsCode = string.Join(",", from p in method.ParameterList.Parameters select p.Identifier.ToFullString());
                            
                            typesOfSerialization.AddRange(from p in method.ParameterList.Parameters select p.Type);
                            var method1 = method.WithBody(SyntaxFactory.Block(SyntaxFactory.ParseStatement(
                                $@"
                                    var info = typeof({interfaceCode}).GetMethod(""{methodCode}"");
                                    _CallMethodEvent(info , new object[] {{{methodCallParamsCode}}} , null);                    
                                ")));

                            type = type.ReplaceNode(method, method1);                                                        
                        }
                    }
                    else if (returnType is QualifiedNameSyntax qn)
                    {
                        if(qn.Left.ToString() == "Regulus.Remote")
                        {
                            
                        }
                    }
                }
                
            }
            TypesOfSerialization = typesOfSerialization; 
            Type = type;

        }
        

    }


}