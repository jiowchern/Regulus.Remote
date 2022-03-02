using Microsoft.CodeAnalysis;

namespace Regulus.Remote.Tools.Protocol.Sources
{
    public class EssentialReference
    {
        public readonly INamedTypeSymbol RegulusRemoteProtocolCreatorAttribute;
        public readonly INamedTypeSymbol RegulusRemoteProperty;
        public readonly INamedTypeSymbol RegulusRemoteNotifier;
        public readonly INamedTypeSymbol RegulusRemoteValue;

        public EssentialReference(Microsoft.CodeAnalysis.Compilation compilation)
        {

            RegulusRemoteProtocolCreatorAttribute = compilation.GetTypeByMetadataName("Regulus.Remote.Protocol.CreatorAttribute");
            RegulusRemoteProperty = compilation.GetTypeByMetadataName("Regulus.Remote.Property`1");
            RegulusRemoteNotifier = compilation.GetTypeByMetadataName("Regulus.Remote.Notifier`1");
            RegulusRemoteValue = compilation.GetTypeByMetadataName("Regulus.Remote.Value`1");
        }

        
    }
}
