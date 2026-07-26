namespace Ris.Idl.Symbols.DocComment;

/// <summary>
/// The symbol for a documentation comment.
/// </summary>
public class IdlDocCommentSymbol : IIdlSymbol
{
    /// <summary>
    /// The name of the documentation.
    /// </summary>
    public string Name = "DocComment";
    
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

    protected bool Equals(IdlDocCommentSymbol other)
    {
        return Name == other.Name && Summary == other.Summary && Equals(Remarks, other.Remarks) && Equals(Parameters, other.Parameters) && Returns == other.Returns;
    }

    /// <inheritdoc/>
    public bool Equals(IIdlSymbol? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((IdlDocCommentSymbol)obj);
    }
}