using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;

namespace Regulus.Remote.Tools.Protocol.Sources
{
    class Configuration
    {
        public const string Filename = "regulus.remote.configuration.toml";
        

        public struct IdentificationSection
        {
            public string Tag { get; set; }
        }

        public IdentificationSection Identification;

        public Microsoft.CodeAnalysis.INamedTypeSymbol GetTag(Compilation com)
        {
            if (string.IsNullOrEmpty(Identification.Tag))
                return null;

            return com.FindAllInterfaceSymbol(s => _CheckTag(s)).FirstOrDefault();
        }

        private bool _CheckTag(INamedTypeSymbol symbol)
        {
            var name = symbol.ToDisplayString();
            return name  == Identification.Tag;
        }

    }
}
