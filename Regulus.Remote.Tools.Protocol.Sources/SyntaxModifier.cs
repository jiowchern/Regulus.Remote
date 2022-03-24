using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using static Microsoft.CodeAnalysis.SyntaxNodeExtensions;
using Regulus.Remote.Tools.Protocol.Sources.Extensions;
namespace Regulus.Remote.Tools.Protocol.Sources
{
    

    class SyntaxModifier
    {
        public static SyntaxModifier Create(Compilation com)
        {
            return new SyntaxModifier(
                    new BlockModifiers.MethodVoid(com),
                    new BlockModifiers.MethodRegulusRemoteValue(com),
                    new BlockModifiers.EventSystemAction(com),
                    new BlockModifiers.PropertyRegulusRemoteBlock(com),
                    new Modifiers.EventFieldDeclarationSyntax(),
                    new Modifiers.PropertyFieldDeclarationSyntax()
                );
        }
        private readonly BlockModifiers.MethodVoid _MethodVoid;
        private readonly BlockModifiers.MethodRegulusRemoteValue _MethodRegulusRemoteValue;
        private readonly BlockModifiers.EventSystemAction _EventSystemAction;
        private readonly BlockModifiers.PropertyRegulusRemoteBlock _PropertyRegulusRemoteBlock;
        private readonly Modifiers.EventFieldDeclarationSyntax _EventFieldDeclarationSyntax;
        private readonly Modifiers.PropertyFieldDeclarationSyntax _PropertyFieldDeclarationSyntax;

        public SyntaxModifier(
            BlockModifiers.MethodVoid method_void,
            BlockModifiers.MethodRegulusRemoteValue method_regulus_remote_value,
            BlockModifiers.EventSystemAction event_system_action ,
            BlockModifiers.PropertyRegulusRemoteBlock property_regulus_remote_block,
            Modifiers.EventFieldDeclarationSyntax event_field_declaration_syntax,
            Modifiers.PropertyFieldDeclarationSyntax property_field_declaration_syntax
            )
        {
            _MethodVoid = method_void;
            _MethodRegulusRemoteValue = method_regulus_remote_value;
            _EventSystemAction = event_system_action;
            _PropertyRegulusRemoteBlock = property_regulus_remote_block;
            _EventFieldDeclarationSyntax = event_field_declaration_syntax;
            _PropertyFieldDeclarationSyntax = property_field_declaration_syntax;
        }


        public ClassAndTypes Mod(ClassDeclarationSyntax type)
        {
            var methods = type.DescendantNodes().OfType<MethodDeclarationSyntax>();

            type = _ModifyMethodParameters(type, methods);

            var propertys = new System.Collections.Generic.HashSet<PropertyDeclarationSyntax>(SyntaxNodeComparer.Default);
            var events = new System.Collections.Generic.HashSet<EventDeclarationSyntax>(SyntaxNodeComparer.Default);
            var typesOfSerialization = new System.Collections.Generic.HashSet<TypeSyntax>(SyntaxNodeComparer.Default);

            var blocks = type.DescendantNodes().OfType<BlockSyntax>();

            var replaceBlocks = new System.Collections.Generic.Dictionary<BlockSyntax, BlockSyntax>();
            foreach (var block in blocks)
            {
                var nodes = block.GetParentPathAndSelf();

                var esa = _EventSystemAction.Mod(nodes);
                if (esa != null)
                {
                    events.Add(esa.Event);
                    replaceBlocks.Add(block, esa.Block);
                }

                var methodVoid = _MethodVoid.Mod(nodes);
                if (methodVoid != null)
                {
                    typesOfSerialization.AddRange(methodVoid.Types);
                    replaceBlocks.Add(block, methodVoid.Block);
                }

                var mrrv = _MethodRegulusRemoteValue.Mod(nodes);
                if (mrrv != null)
                {
                    typesOfSerialization.AddRange(mrrv.Types);
                    replaceBlocks.Add(block, mrrv.Block);
                }
                var prrb = _PropertyRegulusRemoteBlock.Mod(nodes);
                if (prrb != null)
                {
                    propertys.Add(prrb.Property);
                    replaceBlocks.Add(block, prrb.Block);
                }
            }

            type = _ModifyBlocks(type, replaceBlocks);

            var eventDeclarationSyntaxes = events;

            foreach (var eds in eventDeclarationSyntaxes)
            {
                var efds = _EventFieldDeclarationSyntax.Mod(eds);
                if (efds == null)
                    continue;

                type = type.AddMembers(efds.Field);
                typesOfSerialization.AddRange(efds.Types);
            }

            var propertyDeclarationSyntaxes = propertys;

            foreach (var pds in propertyDeclarationSyntaxes)
            {
                var pfds = _PropertyFieldDeclarationSyntax.Mod(pds);
                if (pfds == null)
                    continue;
                type = type.AddMembers(pfds.Field);
                typesOfSerialization.AddRange(pfds.Types);
            }


            return new ClassAndTypes { Type = type, TypesOfSerialization = typesOfSerialization };

        }

        private static ClassDeclarationSyntax _ModifyBlocks(ClassDeclarationSyntax type, System.Collections.Generic.Dictionary<BlockSyntax, BlockSyntax> replaceBlocks)
        {
            type = type.ReplaceNodes(replaceBlocks.Keys, (n1, n2) =>
            {
                if (replaceBlocks.ContainsKey(n1))
                {
                    return replaceBlocks[n1];
                }
                return n1;
            });
            return type;
        }

        private static ClassDeclarationSyntax _ModifyMethodParameters(ClassDeclarationSyntax type, System.Collections.Generic.IEnumerable<MethodDeclarationSyntax> methods)
        {
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
            return type;
        }
    }


}