using System;
using System.Linq;
using System.Xml.Schema;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Regulus.Remote.Tools.Protocol.Sources
{
    public class SyntaxTreeBuilder
    {
        public readonly SyntaxTree Tree;
        public SyntaxTreeBuilder(SourceText source)
        {
            Tree = SyntaxFactory.ParseSyntaxTree(source);

            
        }

        public System.Collections.Generic.IEnumerable<InterfaceDeclarationSyntax> GetInterfaces(string name)
        {


           
            var ids = from i in Tree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>()
                    where i.Identifier.ToString() == name
                     select i;
                ;

                return ids;
        }
    }
}