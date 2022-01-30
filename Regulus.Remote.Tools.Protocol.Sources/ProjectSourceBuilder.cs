using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Regulus.Remote.Tools.Protocol.Sources
{
    public class ProjectSourceBuilder
    {
        public readonly IEnumerable<SyntaxTree> Sources;
        public ProjectSourceBuilder(Compilation compilation)
        {
            var builder = new GhostBuilder(compilation);

            var extractor = new SerializableExtractor(compilation);
            var eventProviderCodeBuilder = new EventProviderCodeBuilder(builder.Events);

            var interfaceProviderCodeBuilder = new InterfaceProviderCodeBuilder(builder.Ghosts);
            var membermapCodeBuilder = new MemberMapCodeBuilder(compilation);
            var protocol = new ProtocolBuilder(compilation, extractor, eventProviderCodeBuilder, interfaceProviderCodeBuilder, membermapCodeBuilder).Tree;

            Sources = builder.Ghosts.Union(builder.Events).Union(new[] {protocol});
        }
    }
}