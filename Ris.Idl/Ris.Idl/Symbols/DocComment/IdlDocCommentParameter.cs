namespace Ris.Idl.Symbols.DocComment;

/// <summary>
/// The parameter of a doc comment.
/// </summary>
public record IdlDocCommentParameter
{
    /// <summary>
    /// The name of the parameter.
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// The description of the parameter.
    /// </summary>
    public string? Description { get; set; }
}