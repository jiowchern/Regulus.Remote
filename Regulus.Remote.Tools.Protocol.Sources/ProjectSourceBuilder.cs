using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Regulus.Remote.Tools.Protocol.Sources
{
    public class ProjectSourceBuilder
    {
        public readonly IEnumerable<SyntaxTree> Sources;
        public ProjectSourceBuilder(EssentialReference references)
        {
            var compilation = references.Compilation;
            var builder = new GhostBuilder(compilation);

            var extractor = new SerializableExtractor(references, builder.Ghosts);
            var eventProviderCodeBuilder = new EventProviderCodeBuilder(builder.Events);

            var interfaceProviderCodeBuilder = new InterfaceProviderCodeBuilder(builder.Ghosts.Select(g=>g.Syntax));
            var membermapCodeBuilder = new MemberMapCodeBuilder(compilation);
            var protocolBuilder = new ProtocolBuilder(compilation, extractor, eventProviderCodeBuilder, interfaceProviderCodeBuilder, membermapCodeBuilder);
            var protocol = protocolBuilder.Tree;
            var protocolProviders = new ProtocoProviderlBuilder(compilation, protocolBuilder.ProtocolName).Trees;

            Sources = builder.Ghosts.Select(g => g.Syntax).Union(builder.Events).Union(protocolProviders).Union(new[] {protocol });
        }
    }
}