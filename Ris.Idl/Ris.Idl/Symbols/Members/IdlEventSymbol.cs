using Ris.Idl.Symbols.DocComment;

namespace Ris.Idl.Symbols.Members;

/// <summary>
/// The symbol for an event.
/// </summary>
public class IdlEventSymbol
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
    /// The parameters of the method.
    /// </summary>
    public IReadOnlyList<IdlParameterSymbol>? Parameters { get; set; }
}