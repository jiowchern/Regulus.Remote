using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
namespace Regulus.Remote.Tools.Protocol.Sources
{

    public static class EventDeclarationSyntaxExtensions
    {
        public static ClassDeclarationSyntax CreateRegulusRemoteIEventProxyCreater(this EventDeclarationSyntax eds)
        {

            var een = eds.ExplicitInterfaceSpecifier.Name.ToString().Replace('.', '_');

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
            var typesExpression = "";
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
                typesExpression = typeList.ToString();
            }



            var source = $@"
            
            class {className} : Regulus.Remote.IEventProxyCreater
                {{
            
                    System.Type _Type;
                    string _Name;
                    
                    public {className}()
                    {{
                        _Name = ""{fieldName}"";
                        _Type = typeof({typeName});                   
                    
                    }}
                    System.Delegate Regulus.Remote.IEventProxyCreater.Create(long soul_id,int event_id,long handler_id, Regulus.Remote.InvokeEventCallabck invoke_Event)
                    {{                
                        var closure = new Regulus.Remote.GenericEventClosure(soul_id , event_id ,handler_id, invoke_Event);                
                        return new System.Action{typesExpression}(({paramExpression}) => closure.Run(new object[]{{{paramExpression}}}));
                    }}
                
            
                    System.Type Regulus.Remote.IEventProxyCreater.GetType()
                    {{
                        return _Type;
                    }}            
            
                    string Regulus.Remote.IEventProxyCreater.GetName()
                    {{
                        return _Name;
                    }}            
                }}
                        ";

            var tree = CSharpSyntaxTree.ParseText(source);

            return tree.GetRoot().DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().Single();
        }
    }
}