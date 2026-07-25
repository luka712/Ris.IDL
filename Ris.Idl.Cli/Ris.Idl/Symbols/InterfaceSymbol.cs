namespace Ris.Idl.Symbols;

/// <summary>
/// The symbol for an interface.
/// </summary>
public class InterfaceSymbol
{
    /// <summary>
    /// The name of the interface.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// The visibility of the interface.
    /// </summary>
    public Visibility Visibility { get; set; }

    /// <summary>
    /// The properties of the interface.
    /// </summary>
    public List<PropertySymbol> PropertySymbols { get; set; } = new();
}