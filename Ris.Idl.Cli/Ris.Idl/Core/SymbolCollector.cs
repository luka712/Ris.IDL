using Microsoft.CodeAnalysis;
using Ris.Idl.Core.DocComments;
using Ris.Idl.Symbols;
using Ris.Idl.Symbols.Members;

namespace Ris.Idl.Core;

/// <summary>
/// The symbol collector.
/// </summary>
public class SymbolCollector : ISymbolCollector
{
    private readonly DocCommentParser _docCommentParser = new();

    private readonly Dictionary<Accessibility, IdlVisibility> _visibilityMap =
        new Dictionary<Accessibility, IdlVisibility>()
        {
            [Accessibility.Private] = IdlVisibility.PRIVATE,
            [Accessibility.Public] = IdlVisibility.PUBLIC,
            [Accessibility.Protected] = IdlVisibility.PROTECTED,
            [Accessibility.Internal] = IdlVisibility.INTERNAL,
        };

    /// TODO: 
    public IReadOnlyList<IIdlSymbol> CollectSymbols(IEnumerable<INamedTypeSymbol> symbols)
    {
        var symbolsList = new List<IIdlSymbol>();

        foreach (var symbol in symbols)
        {
            var interfaceSymbol = GetInterfaceSymbol(symbol);
            if (interfaceSymbol != null)
            {
                symbolsList.Add(interfaceSymbol);
            }
        }

        return symbolsList;
    }

    private IdlInterfaceSymbol? GetInterfaceSymbol(INamedTypeSymbol symbol)
    {
        if (symbol.TypeKind != TypeKind.Interface)
        {
            return null;
        }

        var interfaceSymbol = new IdlInterfaceSymbol()
        {
            Name = symbol.Name,
            Visibility = _visibilityMap[symbol.DeclaredAccessibility]
        };

        var properties = symbol.GetMembers().OfType<IPropertySymbol>().ToList();
        interfaceSymbol.PropertySymbols = GetProperties(properties);
        
        var methods = symbol.GetMembers().OfType<IMethodSymbol>().ToList();
        interfaceSymbol.MethodSymbols = GetMethods(methods);

        return interfaceSymbol;
    }


    private IReadOnlyList<IdlPropertySymbol> GetProperties(IReadOnlyList<IPropertySymbol> properties)
    {
        List<IdlPropertySymbol> propertiesSymbols = new();

        for (var i = 0; i < properties.Count; i++)
        {
            var property = properties[i];
            var propertySymbol = new IdlPropertySymbol()
            {
                Name = property.Name,
                SymbolType = property.Type.Name,
                IsReadonly = property.IsReadOnly,
                Visibility = _visibilityMap[property.DeclaredAccessibility],
                NullableAnnotation = property.NullableAnnotation == NullableAnnotation.Annotated,
                DocComment = _docCommentParser.GenerateDocComment(property)
            };
            propertiesSymbols.Add(propertySymbol);
        }

        return propertiesSymbols;
    }
    
    private IReadOnlyList<IdlMethodSymbol> GetMethods(IReadOnlyList<IMethodSymbol> methods)
    {
        List<IdlMethodSymbol> methodSymbols = new();

        for (var i = 0; i < methods.Count; i++)
        {
            var method = methods[i];
            var parameters = method.Parameters.Select(x => new IdlMethodParameterSymbol()
            {
                Name = x.Name,
                Type = x.Type.Name,
                DefaultValue = x.HasExplicitDefaultValue ? x.ExplicitDefaultValue?.ToString() : null
            }).ToList();
            
            var methodSymbol = new IdlMethodSymbol()
            {
                Name = method.Name,
                ReturnType = method.ReturnType.Name,
                Visibility = _visibilityMap[method.DeclaredAccessibility],
                DocComment = _docCommentParser.GenerateDocComment(method),
                Parameters = parameters
            };
            methodSymbols.Add(methodSymbol);
        }

        return methodSymbols;
    }
}