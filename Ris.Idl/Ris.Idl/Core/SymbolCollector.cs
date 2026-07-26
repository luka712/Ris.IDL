using System.Collections.Immutable;
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
    private readonly IdGenerator _idGenerator = new ();

    private readonly Dictionary<Accessibility, IdlVisibility> _visibilityMap =
        new Dictionary<Accessibility, IdlVisibility>()
        {
            [Accessibility.Private] = IdlVisibility.PRIVATE,
            [Accessibility.Public] = IdlVisibility.PUBLIC,
            [Accessibility.Protected] = IdlVisibility.PROTECTED,
            [Accessibility.Internal] = IdlVisibility.INTERNAL,
        };

    /// TODO: 
    public IReadOnlyList<IIdlNamedSymbol> CollectSymbols(IEnumerable<INamedTypeSymbol> symbols)
    {
        var symbolsList = new List<IIdlNamedSymbol>();

        foreach (var symbol in symbols)
        {
            var interfaceSymbol = GetInterfaceSymbol(symbol);
            if (interfaceSymbol != null)
            {
                symbolsList.Add(interfaceSymbol);
            }
            
            var enumSymbol = GetEnum(symbol);
            if (enumSymbol != null)
            {
                symbolsList.Add(enumSymbol);
            }
            
            var classSymbol = GetClassSymbol(symbol);
            if (classSymbol != null)
            {
                symbolsList.Add(classSymbol);
            }
            
            var structSymbol = GetStructSymbol(symbol);
            if (structSymbol != null)
            {
                symbolsList.Add(structSymbol);
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

        var interfaceSymbol = new IdlInterfaceSymbol(symbol.Name, symbol.ContainingNamespace.ToDisplayString())
        {
            Visibility = _visibilityMap[symbol.DeclaredAccessibility],
            DocComment = _docCommentParser.GenerateDocComment(symbol)
        };
        
        interfaceSymbol.Inherits = GetInheritedMembers(symbol.Interfaces);

        var properties = symbol.GetMembers().OfType<IPropertySymbol>().ToList();
        interfaceSymbol.PropertySymbols = GetProperties(properties);
        
        var methods = symbol.GetMembers().OfType<IMethodSymbol>().ToList();
        interfaceSymbol.MethodSymbols = GetMethods(methods);
        
        var events = symbol.GetMembers().OfType<IEventSymbol>().ToList();
        interfaceSymbol.EventSymbols = GetEvents(events);

        return interfaceSymbol;
    }
    
    private IdlClassSymbol? GetClassSymbol(INamedTypeSymbol symbol)
    {
        if (symbol.TypeKind != TypeKind.Class)
        {
            return null;
        }

        var classSymbol = new IdlClassSymbol(symbol.Name, symbol.ContainingNamespace.ToDisplayString())
        {
            Visibility = _visibilityMap[symbol.DeclaredAccessibility],
            DocComment = _docCommentParser.GenerateDocComment(symbol)
        };

        var properties = symbol.GetMembers().OfType<IPropertySymbol>().ToList();
        classSymbol.PropertySymbols = GetProperties(properties);
        
        var methods = symbol.GetMembers().OfType<IMethodSymbol>().ToList();
        classSymbol.MethodSymbols = GetMethods(methods);

        return classSymbol;
    }
    
    private IdlStructSymbol? GetStructSymbol(INamedTypeSymbol symbol)
    {
        if (symbol.TypeKind != TypeKind.Struct)
        {
            return null;
        }

        var structSymbol = new IdlStructSymbol(symbol.Name, symbol.ContainingNamespace.ToDisplayString())
        {
            Visibility = _visibilityMap[symbol.DeclaredAccessibility],
            DocComment = _docCommentParser.GenerateDocComment(symbol)
        };

        var properties = symbol.GetMembers().OfType<IPropertySymbol>().ToList();
        structSymbol.PropertySymbols = GetProperties(properties);
        
        var methods = symbol.GetMembers().OfType<IMethodSymbol>().ToList();
        structSymbol.MethodSymbols = GetMethods(methods);

        return structSymbol;
    }

    private IdlEnumSymbol? GetEnum(INamedTypeSymbol symbol)
    {
        if (symbol.TypeKind != TypeKind.Enum)
        {
            return null;
        }

        IdlEnumSymbol enumSymbol = new(symbol.Name, symbol.ContainingNamespace.ToDisplayString())
        {
            DocComment = _docCommentParser.GenerateDocComment(symbol),
            Visibility = _visibilityMap[symbol.DeclaredAccessibility]
        };

        List<IdlEnumFieldSymbol> fieldSymbols = new();

        var fields = symbol.GetMembers().OfType<IFieldSymbol>().ToList();
        foreach (var enumMember in fields)
        {
            var name = enumMember.Name;
            var value = enumMember.ConstantValue?.ToString();
            
            fieldSymbols.Add(new IdlEnumFieldSymbol()
            {
                Name = enumMember.Name,
                DocComment = _docCommentParser.GenerateDocComment(enumMember),
                Value = value,
            });
        }
        
        enumSymbol.FieldSymbols = fieldSymbols;
        
        return enumSymbol;
    }

    private IdlTypeSymbol ResolveType(ITypeSymbol type)
    {
        // Handle array type.
        if (type is IArrayTypeSymbol arrayType)
        { 
            var elementType = ResolveType(arrayType.ElementType);
            
            return new IdlTypeSymbol()
            {
                Name = "Array",
                Namespace = type.ContainingNamespace?.ToDisplayString() ?? "",
                ElementType = elementType,
                IsArray = true
            };
        }
        
        var name = type.Name;
        var @namespace = type.ContainingNamespace.ToDisplayString();
        
        return new IdlTypeSymbol()
        {
            Name = name,
            Namespace = @namespace,
        };
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
                TypeSymbol = ResolveType(property.Type),
                IsReadonly = property.IsReadOnly,
                Visibility = _visibilityMap[property.DeclaredAccessibility],
                NullableAnnotation = property.NullableAnnotation == NullableAnnotation.Annotated,
                DocComment = _docCommentParser.GenerateDocComment(property),
                IsStatic = property.IsStatic,
            };
            propertiesSymbols.Add(propertySymbol);
        }

        return propertiesSymbols;
    }
    
    private IReadOnlyList<IdlMethodSymbol> GetMethods(IReadOnlyList<IMethodSymbol> methods)
    {
        // We only care about ordinary methods.
        methods = methods.Where(x => x.MethodKind == MethodKind.Ordinary).ToList();
        
        List<IdlMethodSymbol> methodSymbols = new();

        for (var i = 0; i < methods.Count; i++)
        {
            var method = methods[i];
          
            var parameters = method.Parameters.Select(x => new IdlParameterSymbol()
            {
                Name = x.Name,
                Type = ResolveType(x.Type),
                DefaultValue = x.HasExplicitDefaultValue ? x.ExplicitDefaultValue?.ToString() : null,
                NullableAnnotation = x.NullableAnnotation == NullableAnnotation.Annotated,
                IsStatic = method.IsStatic,
            }).ToList();
            
            var methodSymbol = new IdlMethodSymbol()
            {
                Name = method.Name,
                ReturnType = ResolveType(method.ReturnType),
                Visibility = _visibilityMap[method.DeclaredAccessibility],
                DocComment = _docCommentParser.GenerateDocComment(method),
                Parameters = parameters
            };
            methodSymbols.Add(methodSymbol);
        }

        return methodSymbols;
    }
    
    private IReadOnlyList<IdlEventSymbol> GetEvents(IReadOnlyList<IEventSymbol> events)
    {
        List<IdlEventSymbol> eventSymbols = new();

        for (var i = 0; i < events.Count; i++)
        {
            var @event = events[i];
            var parameters = new List<IdlParameterSymbol>();
            if (@event.Type is INamedTypeSymbol delegateType)
            {
                var invokeMethod = delegateType.DelegateInvokeMethod;

                if (invokeMethod != null)
                {
                    parameters = invokeMethod.Parameters
                        .Select(x => new IdlParameterSymbol()
                        {
                            Name = x.Name,
                            Type = ResolveType(x.Type),
                            DefaultValue = x.HasExplicitDefaultValue 
                                ? x.ExplicitDefaultValue?.ToString() 
                                : null,
                            NullableAnnotation = x.NullableAnnotation == NullableAnnotation.Annotated,
                        })
                        .ToList();
                }
            }
            
            var eventSymbol = new IdlEventSymbol()
            {
                // Event methods will have name starting with "add_", for example, "add_OnClick".
                Name = @event.Name.Replace("add_", ""),
                Visibility = _visibilityMap[@event.DeclaredAccessibility],
                DocComment = _docCommentParser.GenerateDocComment(@event),
                Parameters = parameters
            };
            eventSymbols.Add(eventSymbol);
        }

        return eventSymbols;
    }
    
    private IReadOnlyList<IdlNamedSymbol> GetInheritedMembers(ImmutableArray<INamedTypeSymbol> inherits)
    {
        List<IdlNamedSymbol> inheritedSymbols = new();

        for (var i = 0; i < inherits.Length; i++)
        {
            var @inherit = inherits[i];
          
            inheritedSymbols.Add(new IdlNamedSymbol(@inherit.Name, @inherit.ContainingNamespace.ToDisplayString()));
        }

        return inheritedSymbols;
    }
}