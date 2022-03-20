using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
namespace Regulus.Remote.Tools.Protocol.Sources
{
    class TypeSyntaxEqualityComparer : IEqualityComparer<TypeSyntax>
    {
        bool IEqualityComparer<TypeSyntax>.Equals(TypeSyntax x, TypeSyntax y)
        {
            var b = x.IsEquivalentTo(y);
            return b;
        }

        int IEqualityComparer<TypeSyntax>.GetHashCode(TypeSyntax obj)
        {
            
            return obj.ToFullString().GetHashCode();
        }
    }

}