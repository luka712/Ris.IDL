namespace Ris.Idl.Symbols;

/// <summary>
/// The symbol for a property.
/// </summary>
public class PropertySymbol
{
    /// <summary>
    /// The name of the property.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// The type of the property.
    /// </summary>
    public Type SymbolType { get; set; }
    
    /// <summary>
    /// The visibility of the property.
    /// </summary>
    public Visibility Visibility { get; set; }
    
    /// <summary>
    /// Whether the property is optional.
    /// </summary>
    public bool IsOptional { get; set; }
    
    /// <summary>
    /// Indicates whether the property is readonly.
    /// </summary>
    public bool IsReadonly { get; set; }
}