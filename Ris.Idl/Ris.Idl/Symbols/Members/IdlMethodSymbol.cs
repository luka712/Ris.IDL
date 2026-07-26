using Ris.Idl.Symbols.DocComment;

namespace Ris.Idl.Symbols.Members;

/// <summary>
/// The symbol for a property.
/// </summary>
public record IdlMethodSymbol
{
    /// <summary>
    /// The name of the property.
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// The documentation comment for the property.
    /// </summary>
    public IdlDocCommentSymbol? DocComment { get; set; }
    
    /// <summary>
    /// The visibility of the property.
    /// </summary>
    public IdlVisibility Visibility { get; set; } = IdlVisibility.PRIVATE;
    
    /// <summary>
    /// Indicates whether the property is nullable.
    /// </summary>
    public bool NullableAnnotation { get; set; }
    
    /// <summary>
    /// The return type of the method.
    /// </summary>
    public required IdlTypeSymbol ReturnType { get; init; }
    
    /// <summary>
    /// The parameters of the method.
    /// </summary>
    public IReadOnlyList<IdlParameterSymbol>? Parameters { get; set; }
    
    /// <summary>
    /// Indicates whether the parameter is a static parameter.
    /// </summary>
    public bool IsStatic { get; set; }
}