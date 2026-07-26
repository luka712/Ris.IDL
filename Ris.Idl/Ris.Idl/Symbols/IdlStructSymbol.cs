using Ris.Idl.Symbols.Members;

namespace Ris.Idl.Symbols;

/// <summary>
/// The symbol for a class.
/// </summary>
public class IdlStructSymbol : IdlNamedSymbol
{
    /// <summary>
    /// The constructor.
    /// </summary>
    /// <param name="name">The name of the class.</param>
    /// <param name="namespace">The class namespace.</param>   
    public IdlStructSymbol(string name, string @namespace)
        :base(name, @namespace)
    {
    }
    
    /// <summary>
    /// The interfaces that the interface inherits from.
    /// </summary>
    public IReadOnlyList<IIdlNamedSymbol>? Inherits { get; set; }

    /// <summary>
    /// The properties of the class.
    /// </summary>
    public IReadOnlyList<IdlPropertySymbol>? PropertySymbols { get; set; }

    /// <summary>
    /// The methods of the class.
    /// </summary>
    public IReadOnlyList<IdlMethodSymbol>? MethodSymbols { get; set; }
    
    /// <summary>
    /// The events of the interface.
    /// </summary>
    public IReadOnlyList<IdlEventSymbol>? EventSymbols { get; set; }
}