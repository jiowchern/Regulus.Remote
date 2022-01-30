using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Regulus.Remote.Tools.Protocol.Sources
{
    internal class ProtocoNewlBuilder
    {
        private MethodDeclarationSyntax syntax;
        private SemanticModel model;

        public ProtocoNewlBuilder(MethodDeclarationSyntax syntax, SemanticModel model)
        {
            this.syntax = syntax;
            this.model = model;
        }
    }
}