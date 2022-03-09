using Microsoft.CodeAnalysis;

namespace Regulus.Remote.Tools.Protocol.Sources
{
    public class EssentialReference
    {
        public readonly INamedTypeSymbol RegulusRemoteProtocolCreaterAttribute;
        public readonly INamedTypeSymbol RegulusRemoteProperty;
        public readonly INamedTypeSymbol RegulusRemoteNotifier;
        public readonly INamedTypeSymbol RegulusRemoteValue;
        public readonly INamedTypeSymbol[] SystemActions;
        public readonly Compilation Compilation;

        public EssentialReference(Microsoft.CodeAnalysis.Compilation compilation)
        {
            
            RegulusRemoteProtocolCreaterAttribute = compilation.GetTypeByMetadataName("Regulus.Remote.Protocol.CreaterAttribute");
            RegulusRemoteProperty = compilation.GetTypeByMetadataName("Regulus.Remote.Property`1");
            RegulusRemoteNotifier = compilation.GetTypeByMetadataName("Regulus.Remote.Notifier`1");
            RegulusRemoteValue = compilation.GetTypeByMetadataName("Regulus.Remote.Value`1");

            
            SystemActions = new[]
            {
                compilation.GetTypeByMetadataName("System.Action"),                
                compilation.GetTypeByMetadataName("System.Action`1"),
                compilation.GetTypeByMetadataName("System.Action`2"),
                compilation.GetTypeByMetadataName("System.Action`3"),
                compilation.GetTypeByMetadataName("System.Action`4"),
                compilation.GetTypeByMetadataName("System.Action`5"),
                compilation.GetTypeByMetadataName("System.Action`6"),
                compilation.GetTypeByMetadataName("System.Action`7"),
                compilation.GetTypeByMetadataName("System.Action`8"),
                compilation.GetTypeByMetadataName("System.Action`9"),
                compilation.GetTypeByMetadataName("System.Action`10"),
                compilation.GetTypeByMetadataName("System.Action`11"),
                compilation.GetTypeByMetadataName("System.Action`12"),
                compilation.GetTypeByMetadataName("System.Action`13"),
                compilation.GetTypeByMetadataName("System.Action`14"),
                compilation.GetTypeByMetadataName("System.Action`15"),
                compilation.GetTypeByMetadataName("System.Action`16"),


            };
            this.Compilation = compilation;
        }

        
    }
}
