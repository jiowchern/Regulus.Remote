using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Regulus.Remote.Tools.Protocol.Sources
{
    public class ProjectSourceBuilder
    {
        public readonly IEnumerable<SyntaxTree> Sources;

        public ProjectSourceBuilder(EssentialReference references)
        {
            var compilation = references.Compilation;
            

            var ghostBuilder = new GhostBuilder(compilation.FindAllInterfaceSymbol());


            var name = System.Guid.NewGuid().ToString().Replace("-", "");
            
            var root = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName($"NS{name}"));
            
            var eventProxys= ghostBuilder.EventProxys.Select(e => CSharpSyntaxTree.ParseText(root.AddMembers(e).NormalizeWhitespace().ToString(),null,$"{name}.{e.Identifier}.cs"));
            var ghosts  = ghostBuilder.Ghosts.Select(e => CSharpSyntaxTree.ParseText(root.AddMembers(e).NormalizeWhitespace().ToString(), null, $"{name}.{e.Identifier}.cs"));

            var eventProviderCodeBuilder = new EventProviderCodeBuilder(eventProxys);


            var interfaceProviderCodeBuilder = new InterfaceProviderCodeBuilder(ghosts);
            var membermapCodeBuilder = new MemberMapCodeBuilder(ghostBuilder.Souls);
            var protocolBuilder = new ProtocolBuilder(compilation, eventProviderCodeBuilder, interfaceProviderCodeBuilder, membermapCodeBuilder, ghostBuilder.Types);

            var protocolProviders = new ProtocoProviderlBuilder(compilation, protocolBuilder.ProtocolName).Trees;

            Sources = ghosts.Union(eventProxys).Union(protocolProviders).Union(new[] { protocolBuilder.Tree });
        }
    }
}