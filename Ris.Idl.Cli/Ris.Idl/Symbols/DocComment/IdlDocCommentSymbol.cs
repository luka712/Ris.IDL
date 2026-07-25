namespace Ris.Idl.Symbols.DocComment;

/// <summary>
/// The symbol for a documentation comment.
/// </summary>
public class IdlDocCommentSymbol : IIdlSymbol
{
    /// <summary>
    /// The summary of the documentation.
    /// </summary>
    public string? Summary { get; set; } 
    
    /// <summary>
    /// The remarks of the documentation.
    /// </summary>
    public IReadOnlyList<string>? Remarks { get; set; }
    
    /// <summary>
    /// The parameters of the documentation.
    /// </summary>
    public IReadOnlyList<IdlDocCommentParameter>? Parameters { get; set; }
    
    /// <summary>
    /// The return value of the documentation.
    /// </summary>
    public string? Returns { get; set; }
}