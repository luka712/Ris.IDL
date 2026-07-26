using Ris.Idl.Symbols.DocComment;

namespace Ris.Idl.Symbols;

/// <summary>
/// The base interface for all symbols.
/// </summary>
public interface IIdlNamedSymbol : IIdlSymbol
{
    /// <summary>
    /// The ID of the symbol.
    /// </summary>
    public string Id { get; }
    
    /// <summary>
    /// The name of the symbol.
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// The documentation comment.
    /// </summary>
    public IdlDocCommentSymbol? DocComment { get; set; }
    
    /// <summary>
    /// The namespace of the symbol.
    /// </summary>
    public string Namespace { get; }
    
    /// <summary>
    /// The visibility of the interface.
    /// </summary>
    public IdlVisibility Visibility { get; set; }
}