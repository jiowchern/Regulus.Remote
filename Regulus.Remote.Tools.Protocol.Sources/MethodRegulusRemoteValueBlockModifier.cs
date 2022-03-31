using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Regulus.Remote.Tools.Protocol.Sources.Extensions;
using System.Linq;

namespace Regulus.Remote.Tools.Protocol.Sources.BlockModifiers
{
    
    internal class MethodRegulusRemoteValue
    {
        private readonly Compilation _Compilation;

        public MethodRegulusRemoteValue(Compilation compilation)
        {
            this._Compilation = compilation;
        }

        public BlockAndTypes Mod(System.Collections.Generic.IEnumerable<SyntaxNode> nodes)
        {
            var block = nodes.Skip(0).FirstOrDefault() as BlockSyntax;
            var md = nodes.Skip(1).FirstOrDefault() as MethodDeclarationSyntax;
            var cd = nodes.Skip(2).FirstOrDefault() as ClassDeclarationSyntax;
            

            if (Extensions.SyntaxExtensions.AnyNull(block, md,cd))
            {
                return null;
            }
            if (!_Compilation.AllSerializable(md.ParameterList.Parameters.Select(p => p.Type)))
                return null;
            if ((from p in md.ParameterList.Parameters
                 from m in p.Modifiers
                 where m.IsKind(SyntaxKind.OutKeyword)
                 select m).Any())
                return null;

            var interfaceCode = md.ExplicitInterfaceSpecifier.Name.ToFullString();
            var methodCode = md.Identifier.ToFullString();
            var methodCallParamsCode = string.Join(",", from p in md.ParameterList.Parameters select p.Identifier.ToFullString());
            var returnType = md.ReturnType;
            var qn = returnType as QualifiedNameSyntax;

            if(qn == null)
                return null;

            if (qn.Left.ToString() != "Regulus.Remote")
                return null;

            var gn = qn.Right as GenericNameSyntax;
            if (gn == null)
                return null;

            if (gn.Identifier.ToString() != "Value")
                return null;

            return new BlockAndTypes
            {
                Block = SyntaxFactory.Block(SyntaxFactory.ParseStatement(
                                $@"
var returnValue = new {returnType}();
var info = typeof({interfaceCode}).GetMethod(""{methodCode}"");

this._CallMethodEvent(info , new object[] {{{methodCallParamsCode}}} , returnValue);                    
return returnValue;
                                            ")),
                Types = (from p in md.ParameterList.Parameters select p.Type).Union(gn.TypeArgumentList.Arguments)
            };
            
        }
    }
}