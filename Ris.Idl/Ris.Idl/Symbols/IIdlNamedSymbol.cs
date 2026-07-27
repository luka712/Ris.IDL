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
    string Id { get; }
    
    /// <summary>
    /// The name of the symbol.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// The type of the symbol.
    /// </summary>
    string Type { get; }
    
    /// <summary>
    /// The documentation comment.
    /// </summary>
    IdlDocCommentSymbol? DocComment { get; set; }
    
    /// <summary>
    /// The namespace of the symbol.
    /// </summary>
    string Namespace { get; }
    
    /// <summary>
    /// The visibility of the interface.
    /// </summary>
    IdlVisibility Visibility { get; set; }
}