using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;

namespace Regulus.Remote.Tools.Protocol.Sources
{
    internal class ProtocolBuilder
    {

        public readonly SyntaxTree Tree;
        public readonly string ProtocolName;
        
        public ProtocolBuilder(
            Compilation compilation,
            SerializableExtractor extractor,
            EventProviderCodeBuilder event_provider_code_builder,
            InterfaceProviderCodeBuilder interface_provider_code_builder,
            MemberMapCodeBuilder membermap_code_builder)
        {

            


           
            var types = extractor.Symbols.Select(s =>
                $"typeof({s.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)})");
            var serCode =string.Join(",", new HashSet<string>(types));

            var md5 = _BuildMd5(serCode + event_provider_code_builder.Code + interface_provider_code_builder.Code +membermap_code_builder.PropertyInfosCode + membermap_code_builder.EventInfosCode + membermap_code_builder.InterfacesCode + membermap_code_builder.MethodInfosCode);

            var protocolName = _BuildProtocolName(md5);
            var verCode = _BuildVerificationCode(md5);
            string code =$@"
using System;  
using System.Collections.Generic;
using Regulus.Remote;
public class {protocolName} : Regulus.Remote.IProtocol
{{
  
    readonly Regulus.Remote.InterfaceProvider _InterfaceProvider;
    readonly Regulus.Remote.EventProvider _EventProvider;
    readonly Regulus.Remote.MemberMap _MemberMap;
    
    readonly System.Reflection.Assembly _Base;
    readonly System.Type[] _SerializeTypes;
    public {protocolName}()
    {{
        _Base = System.Reflection.Assembly.Load(""{compilation.Assembly}"");
       
        _InterfaceProvider = new Regulus.Remote.InterfaceProvider(new Dictionary<Type, Type> (){{ {interface_provider_code_builder.Code}}});
   
        _EventProvider = new Regulus.Remote.EventProvider( new IEventProxyCreater[]{{ {event_provider_code_builder.Code} }});        
        _SerializeTypes = new System.Type[] {{{serCode}}};
        _MemberMap = new Regulus.Remote.MemberMap(
            new System.Reflection.MethodInfo[] {{{membermap_code_builder.MethodInfosCode}}} ,
            new System.Reflection.EventInfo[]{{ {membermap_code_builder.EventInfosCode}}}, 
            new System.Reflection.PropertyInfo[] {{{membermap_code_builder.PropertyInfosCode}}}, 
            new System.Tuple<System.Type, System.Func<Regulus.Remote.IProvider>>[] {{{membermap_code_builder.InterfacesCode}}});
    }}
    
    System.Reflection.Assembly Regulus.Remote.IProtocol.Base =>_Base;
    System.Type[] Regulus.Remote.IProtocol.SerializeTypes => _SerializeTypes;


    byte[] Regulus.Remote.IProtocol.VerificationCode {{ get {{ return new byte[]{{{verCode}}};}} }}
    Regulus.Remote.InterfaceProvider Regulus.Remote.IProtocol.GetInterfaceProvider()
    {{
        return _InterfaceProvider;
    }}

    Regulus.Remote.EventProvider Regulus.Remote.IProtocol.GetEventProvider()
    {{
        return _EventProvider;
    }}

   

    Regulus.Remote.MemberMap Regulus.Remote.IProtocol.GetMemberMap()
    {{
        return _MemberMap;
    }}  
    
}}
            
";

            ProtocolName = protocolName;
           Tree = SyntaxFactory.ParseSyntaxTree(code, null, $"RegulusRemoteProtocol.{protocolName}.cs", Encoding.UTF8);

        }
        private string _BuildProtocolName(byte[] code)
        {
            return $"C{BitConverter.ToString(code).Replace("-", "")}";
        }
        private string _BuildVerificationCode(byte[] code)
        {
            
            return string.Join(",", code.Select(val => val.ToString()).ToArray());
        }

        private byte[] _BuildMd5(string codes)
        {
            MD5 md5 = MD5.Create();
            return md5.ComputeHash(Encoding.ASCII.GetBytes(codes));
        }
    }
}