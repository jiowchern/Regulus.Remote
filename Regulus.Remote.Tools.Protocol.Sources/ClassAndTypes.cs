using Microsoft.CodeAnalysis.CSharp.Syntax;
namespace Regulus.Remote.Tools.Protocol.Sources
{
    public struct ClassAndTypes
    {
        public System.Collections.Generic.IEnumerable<TypeSyntax> TypesOfSerialization;
        public ClassDeclarationSyntax Type;
    }


}