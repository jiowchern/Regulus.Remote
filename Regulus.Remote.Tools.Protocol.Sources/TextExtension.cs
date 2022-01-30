using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Regulus.Remote.Tools.Protocol.Sources
{
    public static class TextExtension
    {
        public static string ToNormalizeWhitespace(this string str)
        {
            return SyntaxFactory.ParseSyntaxTree(str, null, "", System.Text.Encoding.UTF8).GetRoot().NormalizeWhitespace().ToFullString(); ;
            
        }

        public static SourceText ToNormalizeWhitespace(this SyntaxTree str)
        {
            return str.GetRoot().NormalizeWhitespace().GetText(System.Text.Encoding.UTF8) ;
        }

        public static string ToNamespaceTypeName(this InterfaceDeclarationSyntax node)
        {
            var nss = node.Ancestors().OfType<NamespaceDeclarationSyntax>();
            var namespaceDel = nss.Count() == 1 ? $"{nss.Single().Name}." : "";
            return  $"{namespaceDel}{node.Identifier}";
        }
    }
}