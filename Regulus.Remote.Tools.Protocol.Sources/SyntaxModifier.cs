using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using static Microsoft.CodeAnalysis.SyntaxNodeExtensions;
using Regulus.Remote.Tools.Protocol.Sources.Extensions;
namespace Regulus.Remote.Tools.Protocol.Sources
{
    public class SyntaxModifier
    {        
        public readonly System.Collections.Generic.IEnumerable<TypeSyntax> TypesOfSerialization;
        public readonly ClassDeclarationSyntax Type;
        public SyntaxModifier(ClassDeclarationSyntax type)
        {
            

            var blocks = type.DescendantNodes().OfType<BlockSyntax>();

            var typesOfSerialization = new System.Collections.Generic.List<TypeSyntax>();

            var replaceBlocks = new System.Collections.Generic.Dictionary<BlockSyntax, BlockSyntax>();
            foreach (var block in blocks)
            {
                var nodes = block.GetParentPathAndSelf();
                var e = BlockModifiers.Event.Mod(nodes);
                if (e != null)
                {
                    typesOfSerialization.AddRange(e.Types);
                    replaceBlocks.Add( block, e.Block);                    
                }
                var methodVoid = BlockModifiers.MethodVoid.Mod(nodes);
                if (methodVoid != null)
                {
                    typesOfSerialization.AddRange(methodVoid.Types);
                    replaceBlocks.Add(block, methodVoid.Block);
                }

                var mrrv = BlockModifiers.MethodRegulusRemoteValue.Mod(nodes);
                if (mrrv != null)
                {
                    typesOfSerialization.AddRange(mrrv.Types);
                    replaceBlocks.Add(block, mrrv.Block);
                }
            }

            type = type.ReplaceNodes(replaceBlocks.Keys,(n1,n2) =>
            {
                if(replaceBlocks.ContainsKey(n1))
                {
                    return replaceBlocks[n1];
                }
                return n1;
            } );
            TypesOfSerialization = typesOfSerialization;
            Type = type;

        }

     
     

    }


}