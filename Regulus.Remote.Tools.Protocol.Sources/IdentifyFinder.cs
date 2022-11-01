using Microsoft.CodeAnalysis;
using System.Linq;
namespace Regulus.Remote.Tools.Protocol.Sources
{
    class IdentifyFinder
    {
        public readonly INamedTypeSymbol Tag;
        public IdentifyFinder(Compilation compilation)
        {
            Tag = compilation.GetTypeByMetadataName("Regulus.Remote.Protocolable");
        }
    }
}
