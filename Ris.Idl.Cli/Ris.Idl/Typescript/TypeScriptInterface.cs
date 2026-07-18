using Ris.Idl.Interfaces;

namespace Ris.Idl.Typescript;

/// <summary>
/// The TypeScript interface.
/// </summary>
public class TypeScriptInterface : IInterface
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

    public string? FilePath { get; set; }
}