using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;




namespace Regulus.Remote.Tools.Protocol.Sources
{
    
    using Regulus.Remote.Tools.Protocol.Sources.Extensions;
    using System.Linq;

    class MemberMapCodeBuilder
    {
        
        public readonly string MethodInfosCode;
        public readonly string EventInfosCode;
        public readonly string PropertyInfosCode;
        public readonly string InterfacesCode;
        public MemberMapCodeBuilder(IEnumerable<InterfaceDeclarationSyntax> _interfaces)
        {            
            var methods = from interfaceSyntax in _interfaces
                          from methodSyntax in interfaceSyntax.DescendantNodes().OfType<MethodDeclarationSyntax>()
                          select _BuildCode(interfaceSyntax,methodSyntax);

            MethodInfosCode = string.Join(",", methods);

            var events = 
                         from interfaceSyntax in _interfaces
                         from eventSyntax in interfaceSyntax.DescendantNodes().OfType<EventFieldDeclarationSyntax>()
                         select _BuildCode(interfaceSyntax, eventSyntax);

            EventInfosCode = string.Join(",", events);

            var propertys = from interfaceSyntax in _interfaces
                            from propertySyntax in interfaceSyntax.DescendantNodes().OfType<PropertyDeclarationSyntax>()
                            select _BuildCode(interfaceSyntax, propertySyntax);

            PropertyInfosCode = string.Join(",", propertys);


            var interfaces = 
                             from interfaceSyntax in _interfaces

                             select _BuildCode(interfaceSyntax);

            InterfacesCode = string.Join(",", interfaces);
        }

        private string _BuildCode(InterfaceDeclarationSyntax interface_syntax)
        {            
            string typeName = interface_syntax.GetNamePath();
            return
                $@"new System.Tuple<System.Type, System.Func<Regulus.Remote.IProvider>>(typeof({typeName}),()=>new Regulus.Remote.TProvider<{typeName}>())";
        }


        private string _BuildCode(InterfaceDeclarationSyntax interface_syntax,PropertyDeclarationSyntax property_syntax)
        {
            string typeName = interface_syntax.GetNamePath() ;
            string eventName = property_syntax.Identifier.ToString();
            return $@"typeof({typeName}).GetProperty(""{eventName}"")";
        }

        private string _BuildCode(InterfaceDeclarationSyntax interface_syntax, EventFieldDeclarationSyntax event_syntax)
        {

            string typeName = interface_syntax.GetNamePath();
            string eventName = event_syntax.Declaration.Variables[0].ToFullString();
            return $@"typeof({typeName}).GetEvent(""{eventName}"")";

        }


        private string _BuildCode(InterfaceDeclarationSyntax interface_syntax, MethodDeclarationSyntax method_syntax)
        {

            string typeName = interface_syntax.GetNamePath();
            
            var paramNames = method_syntax.ParameterList.Parameters.Count.GetSeries().Select(n=>$"_{n+1}");
            var paramTypes = method_syntax.ParameterList.Parameters.Select(p => p.Type.ToString());

            
            var typeAndParamTypes = string.Join(",", (new[] { typeName }).Concat(paramTypes));
            var instanceAndParamNames = string.Join(",", (new[] { "ins" }).Concat(paramNames));
            var paramNamesStr = string.Join(",", paramNames);
            return $"new Regulus.Utility.Reflection.TypeMethodCatcher((System.Linq.Expressions.Expression<System.Action<{typeAndParamTypes}>>)(({instanceAndParamNames}) => ins.{method_syntax.Identifier}({paramNamesStr}))).Method";

        }
    }
}