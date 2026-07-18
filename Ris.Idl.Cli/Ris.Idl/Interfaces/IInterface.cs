namespace Ris.Idl.Interfaces;

/// <summary>
/// The interface.
/// </summary>
public interface IInterface
{
    /// <summary>
    /// The source code for this interface.
    /// </summary>
    public string? SourceCode { get; set; }
    
    /// <summary>
    /// The name of the interface.
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// The namespace of the interface.
    /// </summary>
    public string? Namespace { get; set; }
    
    /// <summary>
    /// The file path in source code.
    /// </summary>
    public string? FilePath { get; set; }
}