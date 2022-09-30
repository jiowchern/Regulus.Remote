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

            var ghostBuilder = new GhostBuilder(SyntaxModifier.Create(compilation) ,compilation.FindAllInterfaceSymbol(references.RegulusRemoteProtocolable));

            var name = ghostBuilder.Namespace;
            
            var root = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName($"NS{name}"));
            
            var eventProxys= ghostBuilder.EventProxys.Select(e => CSharpSyntaxTree.ParseText(root.AddMembers(e).NormalizeWhitespace().ToString(),null,$"{name}.{e.Identifier}.cs", System.Text.Encoding.UTF8));
            var ghosts  = ghostBuilder.Ghosts.Select(e => CSharpSyntaxTree.ParseText(root.AddMembers(e).NormalizeWhitespace().ToString(), null, $"{name}.{e.Identifier}.cs", System.Text.Encoding.UTF8));

            var eventProviderCodeBuilder = new EventProviderCodeBuilder(eventProxys);


            var interfaceProviderCodeBuilder = new InterfaceProviderCodeBuilder(ghosts);
            var membermapCodeBuilder = new MemberMapCodeBuilder(ghostBuilder.Souls);

            
            var protocolBuilder = new ProtocolBuilder(compilation, eventProviderCodeBuilder, interfaceProviderCodeBuilder, membermapCodeBuilder, new  SerializableExtractor(ghostBuilder.Types));

            var protocolProviders = new ProtocoProviderlBuilder(compilation, protocolBuilder.ProtocolName).Trees;

            Sources = _UniteFilePath(name,ghosts.Concat(eventProxys).Concat(protocolProviders).Concat(new[] { protocolBuilder.Tree }));
        }

        private IEnumerable<SyntaxTree> _UniteFilePath(string name,IEnumerable<SyntaxTree> sources)
        {
            int index = 0;
            foreach (var source in sources)
            {
                yield return source.WithFilePath($"{name}.{index++}.cs");
            }
        }
    }
}