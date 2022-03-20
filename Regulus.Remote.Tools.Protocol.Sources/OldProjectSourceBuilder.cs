using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Regulus.Remote.Tools.Protocol.Sources
{
    public class OldProjectSourceBuilder
    {
        public readonly IEnumerable<SyntaxTree> Sources;
        public OldProjectSourceBuilder(EssentialReference references)
        {
            var compilation = references.Compilation;
            var builder = new OldGhostBuilder(compilation);

            
            var extractor = new SerializableExtractor(references, builder.Ghosts);
            var eventProviderCodeBuilder = new EventProviderCodeBuilder(builder.Events);

            var interfaceProviderCodeBuilder = new InterfaceProviderCodeBuilder(builder.Ghosts.Select(g=>g.Syntax));
            var membermapCodeBuilder = new OldMemberMapCodeBuilder(compilation);
            var protocolBuilder = new OldProtocolBuilder(compilation, extractor, eventProviderCodeBuilder, interfaceProviderCodeBuilder, membermapCodeBuilder);
            var protocol = protocolBuilder.Tree;
            var protocolProviders = new ProtocoProviderlBuilder(compilation, protocolBuilder.ProtocolName).Trees;

            Sources = builder.Ghosts.Select(g => g.Syntax).Union(builder.Events).Union(protocolProviders).Union(new[] {protocol });
        }
    }
}