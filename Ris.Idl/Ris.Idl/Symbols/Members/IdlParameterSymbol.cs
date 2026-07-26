
namespace Ris.Idl.Symbols.Members;

/// <summary>
/// The symbol for a parameter of a method, constructor, or event.
/// </summary>
public record IdlParameterSymbol
{
    /// <summary>
    /// The name of the parameter.
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// The type of the parameter.
    /// </summary>
    public required IdlTypeSymbol Type { get; set; }
    
    /// <summary>
    /// The default value of the parameter.
    /// </summary>
    public object? DefaultValue { get; set; }
    
    /// <summary>
    /// Does the parameter have a nullable annotation?
    /// </summary>
    public bool NullableAnnotation { get; set; }

    /// <summary>
    /// Indicates whether the parameter is a static parameter.
    /// </summary>
    public bool IsStatic { get; set; }
}