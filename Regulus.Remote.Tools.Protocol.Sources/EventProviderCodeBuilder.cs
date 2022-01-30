using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Regulus.Remote.Tools.Protocol.Sources
{
    class EventProviderCodeBuilder
    {
        public readonly string Code;
        public EventProviderCodeBuilder(IEnumerable<SyntaxTree> ghosts)
        {


            var ret = from ghost in ghosts
                from classSyntax in ghost.GetRoot().DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>()
                
                let namespaceSyntax = classSyntax.Ancestors().OfType<NamespaceDeclarationSyntax>().Single()
                select $"new global::{namespaceSyntax.Name}.{classSyntax.Identifier}()";

            Code = string.Join(",", ret);

        }
    }
}