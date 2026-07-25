namespace Ris.Idl.Symbols;

public class IdlProject
{
    /// <summary>
    /// The interfaces in the project.
    /// </summary>
    public IReadOnlyList<IdlInterfaceSymbol>? Interfaces { get; set; }
    
    /// <summary>
    /// The classes in the project.
    /// </summary>
    public IReadOnlyList<IdlClassSymbol>? Classes { get; set; }
}