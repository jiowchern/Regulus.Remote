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
        public struct Moded
        {
            public System.Collections.Generic.IEnumerable<TypeSyntax> TypesOfSerialization;
            public ClassDeclarationSyntax Type;
        }
            

        
        public SyntaxModifier()
        {

        

        }


        public Moded Mod(ClassDeclarationSyntax type)
        {
            var methods = type.DescendantNodes().OfType<MethodDeclarationSyntax>();

            var newParams = new System.Collections.Generic.Dictionary<ParameterSyntax, ParameterSyntax>();
            foreach (var method in methods)
            {
                int idx = 0;
                foreach (var p in method.ParameterList.Parameters)
                {
                    var newP = p.WithIdentifier(SyntaxFactory.Identifier($"_{++idx}"));
                    newParams.Add(p, newP);
                }
            }

            type = type.ReplaceNodes(newParams.Keys, (n1, n2) =>
            {
                if (newParams.ContainsKey(n1))
                {
                    return newParams[n1];
                }
                return n1;
            });

            var typesOfSerialization = new System.Collections.Generic.List<TypeSyntax>();
            var blocks = type.DescendantNodes().OfType<BlockSyntax>();
            var replaceBlocks = new System.Collections.Generic.Dictionary<BlockSyntax, BlockSyntax>();
            foreach (var block in blocks)
            {
                var nodes = block.GetParentPathAndSelf();

                var e = BlockModifiers.Event.Mod(nodes);
                if (e != null)
                {
                    replaceBlocks.Add(block, e.Block);
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
                var prrb = BlockModifiers.PropertyRegulusRemoteBlock.Mod(nodes);
                if (prrb != null)
                {
                    replaceBlocks.Add(block, prrb.Block);
                }
            }

            type = type.ReplaceNodes(replaceBlocks.Keys, (n1, n2) =>
            {
                if (replaceBlocks.ContainsKey(n1))
                {
                    return replaceBlocks[n1];
                }
                return n1;
            });

            var eventDeclarationSyntaxes = type.DescendantNodes().OfType<EventDeclarationSyntax>();

            foreach (var eds in eventDeclarationSyntaxes)
            {
                var efds = Modifiers.EventFieldDeclarationSyntax.Mod(eds);
                if (efds == null)
                    continue;

                type = type.AddMembers(efds.Field);
                typesOfSerialization.AddRange(efds.Types);
            }

            var propertyDeclarationSyntaxes = type.DescendantNodes().OfType<PropertyDeclarationSyntax>();

            foreach (var pds in propertyDeclarationSyntaxes)
            {
                var pfds = Modifiers.PropertyFieldDeclarationSyntax.Mod(pds);
                if (pfds == null)
                    continue;
                type = type.AddMembers(pfds.Field);
                typesOfSerialization.AddRange(pfds.Types);
            }


            return new Moded { Type = type, TypesOfSerialization = typesOfSerialization };
            
        }

    }


}