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
        private readonly Compilation _Compilation;
        private readonly INamedTypeSymbol _RegulusRemoteProperty;

        public SerializableExtractor(Compilation compilation)
        {
            
            _Compilation = compilation;
            _RegulusRemoteProperty = _Compilation.GetTypeByMetadataName("Regulus.Remote.Property`1");

            var symbols = 
                from tree in compilation.SyntaxTrees
                let semanticModel = compilation.GetSemanticModel(tree)
                from node in tree.GetRoot().DescendantNodes()
                let symbol = semanticModel.GetDeclaredSymbol(node) //as INamedTypeSymbol
                where symbol != null
                select symbol;

            //var namedSymbols = symbols.OfType<INamedTypeSymbol>().ToArray();
            var fieldSymbols = symbols.OfType<IFieldSymbol>().ToArray();
            var propertySymbols = symbols.OfType<IPropertySymbol>().ToArray();
            var eventSymbols = symbols.OfType<IEventSymbol>().ToArray();
            var methodSymbols = symbols.OfType<IMethodSymbol>().ToArray();
            var paramSymbols = symbols.OfType<IParameterSymbol>().ToArray();


            var paramNameds=_GetNameds(paramSymbols);
            var methodNameds = _GetNameds(methodSymbols);
            var fieldNameds = _GetNameds(fieldSymbols);

            var eventNameds = _GetNameds(eventSymbols);
            var propertyNameds = _GetNameds(propertySymbols);
            Symbols = new HashSet<ITypeSymbol>(paramNameds.Union(methodNameds).Union(fieldNameds).Union(eventNameds).Union(propertyNameds));

        }
        private IEnumerable<ITypeSymbol> _GetNameds(IEnumerable<IPropertySymbol> symbols)
        {
            return _GetNameds(from symbol in symbols
                let type = symbol.Type as INamedTypeSymbol
                where type != null
                where type != null
                      && _RegulusRemoteProperty == type.OriginalDefinition
                from arg in type.TypeArguments

                select arg);
        }

        private IEnumerable<ITypeSymbol> _GetNameds(IEnumerable<ITypeSymbol> symbols)
        {
            var nameds = symbols.OfType<INamedTypeSymbol>().Where(s=>!(s.IsGenericType || s.IsAbstract)) ;
            var arrays = symbols.OfType<IArrayTypeSymbol>();
            return nameds.Union(_GetNameds(arrays));
        }

        private IEnumerable<ITypeSymbol> _GetNameds(IEnumerable<IArrayTypeSymbol> symbols)
        {
            foreach (var symbol in symbols)
            {
                yield return symbol;
                
                if (symbol is IArrayTypeSymbol arraySymbol)
                {
                    foreach (var typeSymbol in _GetNameds(new[] { arraySymbol.ElementType }))
                    {
                        yield return typeSymbol;
                    }
                }
                else if(symbol is INamedTypeSymbol named)
                {
               
                    yield return named;
                }
                    
            }
        }

        

        private IEnumerable<ITypeSymbol> _GetNameds(IEnumerable<IEventSymbol> event_symbols)
        {
            return _GetNameds(from symbol in event_symbols
                let type = symbol.Type as INamedTypeSymbol
                where type != null
                from typeArgument in type.TypeArguments
              
                select typeArgument);
        }

        IEnumerable<ITypeSymbol> _GetNameds(IEnumerable<IParameterSymbol> args)
        {
            return _GetNameds(from arg in args
                let type = arg.Type 
                   
                    select type);
        }

        IEnumerable<ITypeSymbol> _GetNameds(IEnumerable<IMethodSymbol> symbols)
        {
            return _GetNameds(from symbol in symbols
                   let type = symbol.ReturnType as INamedTypeSymbol

                    where type !=null && _Compilation.GetTypeByMetadataName("Regulus.Remote.Value`1") == type.OriginalDefinition
                   from arg in type.TypeArguments   
                   let argNamed = arg 
                   select argNamed);
        }

        IEnumerable<ITypeSymbol> _GetNameds(IEnumerable<IFieldSymbol> symbols)
        {
            return _GetNameds(from arg in symbols
                   let type = arg.Type 
               
                select type);
        }


    }
}
