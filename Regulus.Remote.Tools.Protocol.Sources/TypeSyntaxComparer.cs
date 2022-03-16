using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Regulus.Remote.Tools.Protocol.Sources
{
    internal class TypeSyntaxComparer : IEqualityComparer<TypeSyntax>
    {
        bool IEqualityComparer<TypeSyntax>.Equals(TypeSyntax x, TypeSyntax y)
        {
            return x.IsEquivalentTo(y);
        }

        int IEqualityComparer<TypeSyntax>.GetHashCode(TypeSyntax obj)
        {
            return obj.GetHashCode();
        }
    }
}