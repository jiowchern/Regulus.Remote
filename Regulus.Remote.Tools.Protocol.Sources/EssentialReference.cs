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
        public readonly DialogProvider Log;

        public EssentialReference(Microsoft.CodeAnalysis.Compilation compilation)
        {
            
            this.Compilation = compilation;
            
            RegulusRemoteProtocolCreaterAttribute = _GetType("Regulus.Remote.Protocol.CreaterAttribute");
            RegulusRemoteProperty = _GetType("Regulus.Remote.Property`1");
            RegulusRemoteNotifier = _GetType("Regulus.Remote.Notifier`1");
            RegulusRemoteValue = _GetType("Regulus.Remote.Value`1");

            
            SystemActions = new[]
            {
                _GetType("System.Action"),
                _GetType("System.Action`1"),
                _GetType("System.Action`2"),
                _GetType("System.Action`3"),
                _GetType("System.Action`4"),
                _GetType("System.Action`5"),
                _GetType("System.Action`6"),
                _GetType("System.Action`7"),
                _GetType("System.Action`8"),
                _GetType("System.Action`9"),
                _GetType("System.Action`10"),
                _GetType("System.Action`11"),
                _GetType("System.Action`12"),
                _GetType("System.Action`13"),
                _GetType("System.Action`14"),
                _GetType("System.Action`15"),
                _GetType("System.Action`16"),
            };
            
        }


        INamedTypeSymbol _GetType(string metaname)
        {
            var type = Compilation.GetTypeByMetadataName(metaname);
            if (type == null)
            {                
                throw new MissingTypeException(metaname);
            }
                

            return type;
        }


    }
}
