using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

public class TypeFieldCollector
{
    private readonly SemanticModel _semanticModel;

    public TypeFieldCollector(SemanticModel semanticModel)
    {
        _semanticModel = semanticModel;
    }

    public List<ITypeSymbol> CollectFields(List<TypeSyntax> typeSyntaxes)
    {
        var collectedTypes = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);
        var result = new List<ITypeSymbol>();

        foreach (var typeSyntax in typeSyntaxes)
        {
            CollectFieldsRecursive(typeSyntax, collectedTypes, result);
        }

        return result;
    }

    private void CollectFieldsRecursive(TypeSyntax typeSyntax, HashSet<ITypeSymbol> collectedTypes, List<ITypeSymbol> result)
    {
        var typeSymbol = _semanticModel.GetTypeInfo(typeSyntax).Type;

        if (typeSymbol != null && !collectedTypes.Contains(typeSymbol))
        {
            collectedTypes.Add(typeSymbol);
            result.Add(typeSymbol);

            var fields = typeSymbol.GetMembers().OfType<IFieldSymbol>().Where(f => f.DeclaredAccessibility == Accessibility.Public);

            foreach (var field in fields)
            {
                var fieldTypeSymbol = field.Type;

                if (fieldTypeSymbol != null)
                {
                    CollectFieldsRecursive(fieldTypeSymbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as TypeSyntax, collectedTypes, result);
                }
            }
        }
    }
}
