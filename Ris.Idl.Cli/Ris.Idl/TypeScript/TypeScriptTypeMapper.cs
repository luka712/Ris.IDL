using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Ris.Idl.TypeScript;

/// <summary>
/// Maps C# types to TypeScript types.
/// </summary>
public class TypeScriptTypeMapper
{
    private readonly ILogger _logger;

    /// <summary>
    /// The constructor.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    public TypeScriptTypeMapper(ILogger logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Converts a C# type symbol to its TypeScript equivalent.
    /// </summary>
    /// <param name="type">The C# type symbol.</param>
    /// <returns>The TypeScript type string.</returns>
    public string MapType(ITypeSymbol type)
    {
        _logger.LogDebug("Mapping type: {Type}", type.ToDisplayString());

        var nullableStr = "";
        
        // Handle nullable types
        if (type.NullableAnnotation == NullableAnnotation.Annotated)
        {
            nullableStr = " | null";
        }

        // Handle arrays
        if (type is IArrayTypeSymbol arrayType)
        {
            return $"{MapType(arrayType.ElementType)}[]";
        }

        // Handle generic types (List<T>, Dictionary<K,V>, etc.)
        if (type is INamedTypeSymbol { IsGenericType: true } genericType)
        {
            return MapGenericType(genericType);
        }

        // Handle special types
        var typeName = type.SpecialType switch
        {
            SpecialType.System_Boolean => "boolean",
            SpecialType.System_Byte => "number",
            SpecialType.System_SByte => "number",
            SpecialType.System_Int16 => "number",
            SpecialType.System_UInt16 => "number",
            SpecialType.System_Int32 => "number",
            SpecialType.System_UInt32 => "number",
            SpecialType.System_Int64 => "number",
            SpecialType.System_UInt64 => "number",
            SpecialType.System_Single => "number",
            SpecialType.System_Double => "number",
            SpecialType.System_Decimal => "number",
            SpecialType.System_Char => "string",
            SpecialType.System_String => "string",
            SpecialType.System_Object => "unknown",
            SpecialType.System_Void => "void",
            _ => MapByTypeName(type)
        };
        
        return typeName + nullableStr;
    }

    /// <summary>
    /// Maps a type by its name when special type handling doesn't apply.
    /// </summary>
    private string MapByTypeName(ITypeSymbol type)
    {
        var fullName = type.ToDisplayString();
        var name = type.Name;

        return name switch
        {
            // Common .NET types
            "DateTime" => "string", // ISO 8601 format
            "DateTimeOffset" => "string",
            "DateOnly" => "string",
            "TimeOnly" => "string",
            "TimeSpan" => "string",
            "Guid" => "string",
            "Uri" => "string",
            
            // Dynamic/object types
            "dynamic" => "any",
            "Object" => "unknown",
            
            // Keep the original name for custom types
            _ => name
        };
    }

    /// <summary>
    /// Maps generic types to TypeScript equivalents.
    /// </summary>
    private string MapGenericType(INamedTypeSymbol genericType)
    {
        var typeName = genericType.Name;
        var typeArgs = genericType.TypeArguments;

        return typeName switch
        {
            // Collections -> arrays
            "List" or "IList" or "ICollection" or "IEnumerable" or "IReadOnlyList" or "IReadOnlyCollection" 
                => $"{MapType(typeArgs[0])}[]",
            
            // HashSet -> arrays (TypeScript doesn't have built-in Set serialization)
            "HashSet" or "ISet" or "IReadOnlySet" 
                => $"{MapType(typeArgs[0])}[]",
            
            // Dictionary -> Record<K, V>
            "Dictionary" or "IDictionary" or "IReadOnlyDictionary" 
                => $"Record<{MapType(typeArgs[0])}, {MapType(typeArgs[1])}>",
            
            // Nullable<T>
            "Nullable" 
                => $"{MapType(typeArgs[0])} | null",
            
            // Task<T> -> T (for API responses)
            "Task" when typeArgs.Length == 1 
                => MapType(typeArgs[0]),
            "Task" 
                => "void",
            
            // ValueTask<T> -> T
            "ValueTask" when typeArgs.Length == 1 
                => MapType(typeArgs[0]),
            "ValueTask" 
                => "void",
            
            // Tuple types
            "Tuple" or "ValueTuple" 
                => $"[{string.Join(", ", typeArgs.Select(MapType))}]",
            
            // KeyValuePair
            "KeyValuePair" 
                => $"{{ key: {MapType(typeArgs[0])}; value: {MapType(typeArgs[1])}; }}",
            
            // Keep generic type with mapped type arguments
            _ => $"{typeName}<{string.Join(", ", typeArgs.Select(MapType))}>"
        };
    }
}
