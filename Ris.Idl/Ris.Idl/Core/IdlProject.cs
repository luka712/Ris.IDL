namespace Ris.Idl.Core;

/// <summary>
/// The project that can be generated.
/// </summary>
public class IdlProject
{
    /// <summary>
    /// The symbols in the project.
    /// </summary>
    public IdlProjectSymbols Symbols { get; set; } = new();
    
    /// <summary>
    /// The generated files.
    /// </summary>
    public IReadOnlyList<GeneratedFile> GeneratedFiles { get; set; } = new List<GeneratedFile>();
}