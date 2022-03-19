using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Regulus.Remote.Tools.Protocol.Sources
{
    internal class ClassDeclarationSyntaxEqualityComparer : IEqualityComparer<ClassDeclarationSyntax>
    {
        bool IEqualityComparer<ClassDeclarationSyntax>.Equals(ClassDeclarationSyntax x, ClassDeclarationSyntax y)
        {
            return x.IsEquivalentTo(y);
        }

        int IEqualityComparer<ClassDeclarationSyntax>.GetHashCode(ClassDeclarationSyntax obj)
        {
            return obj.ToFullString().GetHashCode();
        }
    }
}