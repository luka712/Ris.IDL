using Ris.Idl.Symbols.DocComment;

namespace Ris.Idl.Symbols.Members;

/// <summary>
/// The symbol for a property.
/// </summary>
public record IdlPropertySymbol
{
    /// <summary>
    /// The name of the property.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The type of the property.
    /// </summary>
    public required IdlTypeSymbol TypeSymbol { get; init; }

    /// <summary>
    /// The documentation comment for the property.
    /// </summary>
    public IdlDocCommentSymbol? DocComment { get; set; }

    /// <summary>
    /// The visibility of the property.
    /// </summary>
    public IdlVisibility Visibility { get; set; } = IdlVisibility.PRIVATE;

    /// <summary>
    /// Indicates whether the property is readonly.
    /// </summary>
    public bool IsReadonly { get; set; }
    
    /// <summary>
    /// Indicates whether the property is static.
    /// </summary>
    public bool IsStatic { get; set; }

    /// <summary>
    /// Indicates whether the property is nullable.
    /// </summary>
    public bool NullableAnnotation { get; set; }
}