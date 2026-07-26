namespace Ris.Idl.Symbols.Members;

/// <summary>
/// The symbol for a type.
/// </summary>
public class IdlTypeSymbol
{
    /// <summary>
    /// The name of the type.
    /// </summary>
    public required string Name { get; init; } 
    
    /// <summary>
    /// The namespace of the type.
    /// </summary>
    public required string Namespace { get; init; }
    
    /// <summary>
    /// Indicates whether the type is an array.
    /// </summary>
    public bool IsArray { get; set; }
    
    /// <summary>
    /// The element type if it is an array type.
    /// </summary>
    public IdlTypeSymbol? ElementType { get; set; }
}