using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using static Microsoft.CodeAnalysis.SyntaxNodeExtensions;
namespace Regulus.Remote.Tools.Protocol.Sources
{
    public class SyntaxModifier
    {
        
        public readonly System.Collections.Generic.IEnumerable<TypeSyntax>  TypesOfSerialization;
        public readonly ClassDeclarationSyntax Type;
        public SyntaxModifier(ClassDeclarationSyntax type)
        {
            

            var blocks = type.DescendantNodes().OfType<BlockSyntax>().ToArray();

            var typesOfSerialization = new System.Collections.Generic.List<TypeSyntax>();
            foreach (var block in blocks)
            {
                if(block.Parent is MethodDeclarationSyntax method)
                {
                    type = _Method(type, typesOfSerialization, method);
                }
                else if (block.Parent is AccessorDeclarationSyntax ad)
                {
                    if(ad.Parent is AccessorListSyntax al)
                    {
                        if(al.Parent is EventDeclarationSyntax ed)
                        {
                            if(ad.IsKind(SyntaxKind.AddAccessorDeclaration))
                            {

                            }
                            else if(ad.IsKind(SyntaxKind.RemoveAccessorDeclaration))
                            {

                            }
                        }
                    }
                }
                else
                {

                }

            }

            
            TypesOfSerialization = typesOfSerialization;
            Type = type;

        }

        private static ClassDeclarationSyntax _Method(ClassDeclarationSyntax type, System.Collections.Generic.List<TypeSyntax> typesOfSerialization, MethodDeclarationSyntax method)
        {
            var interfaceCode = method.ExplicitInterfaceSpecifier.Name.ToFullString();
            var methodCode = method.Identifier.ToFullString();
            var methodCallParamsCode = string.Join(",", from p in method.ParameterList.Parameters select p.Identifier.ToFullString());
            var returnType = method.ReturnType;
            typesOfSerialization.AddRange(from p in method.ParameterList.Parameters select p.Type);
            if (returnType is PredefinedTypeSyntax pt)
            {
                if (pt.Keyword.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.VoidKeyword))
                {
                    var method1 = method.WithBody(SyntaxFactory.Block(SyntaxFactory.ParseStatement(
$@"var info = typeof({interfaceCode}).GetMethod(""{methodCode}"");
_CallMethodEvent(info , new object[] {{{methodCallParamsCode}}} , null);")));
                    type = type.ReplaceNode(method, method1);
                }
            }
            else if (returnType is QualifiedNameSyntax qn)
            {
                if (qn.Left.ToString() == "Regulus.Remote")
                {
                    if (qn.Right is GenericNameSyntax gn)
                    {
                        if (gn.Identifier.ToString() == "Value")
                        {
                            var arg = gn.TypeArgumentList.Arguments[0];

                            var method1 = method.WithBody(SyntaxFactory.Block(SyntaxFactory.ParseStatement(
                                                $@"
var returnValue = new {returnType}();
var info = typeof({interfaceCode}).GetMethod(""{methodCode}"");
_CallMethodEvent(info , new object[] {{{methodCallParamsCode}}} , returnValue);                    
return returnValue;
                                                        ")));
                            typesOfSerialization.Add(arg);
                            type = type.ReplaceNode(method, method1);
                        }
                    }
                }
            }

            return type;
        }

    }


}