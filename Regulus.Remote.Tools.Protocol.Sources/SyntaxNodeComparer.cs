using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Regulus.Remote.Tools.Protocol.Sources
{
    class SyntaxNodeComparer : IEqualityComparer<SyntaxNode>
    {
        bool IEqualityComparer<SyntaxNode>.Equals(SyntaxNode x, SyntaxNode y)
        {
            var b = x.IsEquivalentTo(y);
            return b;
        }

        int IEqualityComparer<SyntaxNode>.GetHashCode(SyntaxNode obj)
        {
            
            return obj.ToFullString().GetHashCode();
        }

        public readonly static SyntaxNodeComparer Default = new SyntaxNodeComparer();
    }

}