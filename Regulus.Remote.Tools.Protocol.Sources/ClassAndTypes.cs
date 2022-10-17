using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
namespace Regulus.Remote.Tools.Protocol.Sources
{
    public struct ClassAndTypes
    {
        public System.Collections.Generic.IEnumerable<TypeSyntax> TypesOfSerialization;
        public ClassDeclarationSyntax Type;
        public Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax[] UnprocessedBlocks;        
        
        public System.Collections.Generic.IEnumerable<T> GetSyntaxs<T>()  where T : Microsoft.CodeAnalysis.SyntaxNode
        {
            foreach (var block in UnprocessedBlocks)
            {
                Microsoft.CodeAnalysis.SyntaxNode node = block;

                while(node != null && node.GetType() != typeof(T))
                {
                    node = node.Parent;
                }
                var ret = node as T;
                if(ret != null)
                    yield return ret;
            }            
        }


        


    }


}