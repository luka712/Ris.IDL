using Ris.Idl.Symbols.Members;

namespace Ris.Idl.Symbols;

/// <summary>
/// The symbol for an interface.
/// </summary>
public record IdlInterfaceSymbol : IIdlSymbol
{
    /// <summary>
    /// The name of the interface.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The visibility of the interface.
    /// </summary>
    public IdlVisibility Visibility { get; set; } = IdlVisibility.PRIVATE;

    /// <summary>
    /// The properties of the interface.
    /// </summary>
    public IReadOnlyList<IdlPropertySymbol>? PropertySymbols { get; set; }

    /// <summary>
     /// The methods of the interface.
     /// </summary>
    public IReadOnlyList<IdlMethodSymbol>? MethodSymbols { get; set; }
}