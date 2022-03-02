using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Regulus.Remote.Tools.Protocol.Sources
{
    using System.Linq;
    class SerializableExtractor
    {
        public readonly IReadOnlyCollection<ITypeSymbol> Symbols;
        private readonly EssentialReference _References;

        public SerializableExtractor(EssentialReference references, IEnumerable<GhostBuilder.Ghost> ghosts)
        {



            this._References = references;
            var set = new HashSet<ITypeSymbol>();
            _AddSet(set , _GetValues(ghosts));
            Symbols = set;
         
        }

        private void _AddSet(HashSet<ITypeSymbol> set, IEnumerable<ITypeSymbol> symbols)
        {
            foreach (var symbol in symbols)
            {
                if (symbol.IsAbstract)
                    throw new Exceptions.UnserializableException(symbol);

                if (set.Contains(symbol))
                    continue;

                set.Add(symbol);

                if (symbol.Kind == SymbolKind.ArrayType)
                {
                    var ary = symbol as IArrayTypeSymbol;
                    _AddSet(set , new[] { ary.ElementType });
                }
                else if(symbol.TypeKind == TypeKind.Class || symbol.TypeKind == TypeKind.Struct)
                {
                    var type = symbol as INamedTypeSymbol;                    
                    _AddSet(set, type.GetMembers().OfType<IFieldSymbol>().Select(f=>f.Type));
                }
            }
            
        }

        

        private IEnumerable<ITypeSymbol> _GetValues(IEnumerable<GhostBuilder.Ghost> ghost)
        {
            foreach (var member in ghost.SelectMany(s=>s.GetMembers()))
            {
                if (member.Kind == SymbolKind.Method)
                {
                    var method = member as IMethodSymbol;
                    if (method.MethodKind != MethodKind.Ordinary)
                    {
                        continue;
                    }

                    foreach (var item in _GetTypes(method))
                    {
                        yield return item;
                    }

                }
                else if (member.Kind == SymbolKind.Property)
                {
                    foreach (var item in _GetTypes(member as IPropertySymbol))
                    {
                        yield return item;
                    }
                }
                else if (member.Kind == SymbolKind.Event)
                {
                    foreach (var item in _GetTypes(member as IEventSymbol))
                    {
                        yield return item;
                    }
                }
                else
                {
                    throw new Exceptions.UnserializableException(member);
                }
            }
        }

       

        private IEnumerable<ITypeSymbol> _GetTypes(IEventSymbol event_symbol)
        {
           
            var type = event_symbol.Type as INamedTypeSymbol;
            if (type == null)
                throw new Exceptions.UnserializableException(event_symbol);

            return type.TypeArguments;
        }

        private IEnumerable<ITypeSymbol> _GetTypes(IPropertySymbol property_symbol)
        {                        
            var type = property_symbol.Type as INamedTypeSymbol;
            if (type.OriginalDefinition == _References.RegulusRemoteProperty)
            {
                return type.TypeArguments;
            }
            else if(type.OriginalDefinition == _References.RegulusRemoteNotifier)
            {
                if (type.TypeArguments.Any(t => t.TypeKind != TypeKind.Interface))
                    throw new Exceptions.UnserializableException(property_symbol);
            }
            else
                throw new Exceptions.UnserializableException(property_symbol);

            return new ITypeSymbol[0];
        }

        


        private IEnumerable<ITypeSymbol> _GetTypes(IMethodSymbol method_symbol)
        {
            var retType = method_symbol.ReturnType as INamedTypeSymbol ;
            if(retType == null || retType.OriginalDefinition != _References.RegulusRemoteValue && retType.SpecialType != SpecialType.System_Void) 
            {
                throw new Exceptions.UnserializableException(method_symbol);
            }
            

            foreach (var item in retType.TypeArguments)
            {
                yield return item;
            }

            foreach(var item in method_symbol.Parameters)
            {
                if(item.RefKind != RefKind.None)
                {
                    throw new Exceptions.UnserializableException(method_symbol);
                }
                yield return item.Type;
            }
        }


    }
}
