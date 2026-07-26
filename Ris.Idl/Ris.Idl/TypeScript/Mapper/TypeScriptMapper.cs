using System.Numerics;
using Microsoft.Extensions.Logging;
using Ris.Idl.Symbols;
using Ris.Idl.Symbols.Members;

namespace Ris.Idl.TypeScript.Mapper;

/// <summary>
/// Maps C# types to TypeScript types.
/// </summary>
public class TypeScriptMapper
{
    private readonly ILogger _logger;

    private readonly Dictionary<string, string> _typeMapping = new()
    {
        ["Void"] = "void",
        [nameof(Object)] = "any",
        [nameof(Boolean)] = "boolean",
        [nameof(String)] = "string",
        [nameof(Char)] = "string",
        [nameof(Byte)] = "number",
        [nameof(SByte)] = "number",
        [nameof(Int16)] = "number",
        [nameof(UInt16)] = "number",
        [nameof(Int32)] = "number",
        [nameof(UInt32)] = "number",
        [nameof(Int64)] = "number",
        [nameof(UInt64)] = "number",
        [nameof(Single)] = "number",
        [nameof(Double)] = "number",
        [nameof(Decimal)] = "number",
        
        [nameof(Action)] = "() => void",
        
        // TODO: this should be redefined as mapping
        [nameof(Vector2)] = "vec2",
        [nameof(Vector3)] = "vec3",
        [nameof(Vector4)] = "vec4",
    };

    private readonly Dictionary<IdlVisibility, string> _visibilityMapping = new()
    {
        [IdlVisibility.PUBLIC] = "public",
        [IdlVisibility.PROTECTED] = "protected",
        [IdlVisibility.PRIVATE] = "private",
        [IdlVisibility.INTERNAL] = "public",
    };

    /// <summary>
    /// Key is typed, value is default value.
    /// </summary>
    private readonly Dictionary<string, string> _defaultValueMapping = new()
    {
        ["Void"] = String.Empty,
        [nameof(Boolean)] = "false",
        [nameof(String)] = "\"\"",
    };

    /// <summary>
    /// The constructor.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    public TypeScriptMapper(ILogger logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Converts a C# type symbol to its TypeScript equivalent.
    /// </summary>
    /// <param name="type">The C# type symbol.</param>
    /// <returns>The TypeScript type string.</returns>
    public string MapType(IdlTypeSymbol type)
    {
        if (type.IsArray)
        {
            _logger.LogTrace("Mapping array type:");
            if (type.ElementType is null)
            {
                throw new InvalidOperationException("Array type must have an element type");
            }
            
            var elementTypeName = MapType(type.ElementType);
            return $"{elementTypeName}[]";
        }
        
        var typeName = type.Name;
        var newType = _typeMapping.GetValueOrDefault(typeName, typeName);
        _logger.LogTrace($"Mapping type '{typeName}' to '{newType}'");
        return newType;
    }
    
    /// <summary>
    /// Maps a visibility to a TypeScript visibility.
    /// </summary>
    /// <param name="visibility">The <see cref="IdlVisibility"/>.</param>
    /// <returns>The TypeScript visibility.</returns>
    public string MapVisibility(IdlVisibility visibility)
    {
        return _visibilityMapping[visibility];
    }

    /// <summary>
    /// Maps a default value to a TypeScript default value.
    /// </summary>
    /// <param name="symbol">The <see cref="IdlNamedSymbol"/></param>
    /// <returns>The mapped value.</returns>
    public string GetDefaultValue(IdlTypeSymbol symbol)
    {
        return _defaultValueMapping.GetValueOrDefault(symbol.Name, "null");
    }
}
