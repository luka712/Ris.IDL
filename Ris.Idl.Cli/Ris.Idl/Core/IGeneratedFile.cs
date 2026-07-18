namespace Ris.Idl.Core;

/// <summary>
/// Represents a generated file with its content and path.
/// </summary>
public interface IGeneratedFile
{
    /// <summary>
    /// The generated source code content.
    /// </summary>
    string Content { get; }
    
    /// <summary>
    /// The relative file path where this file should be written.
    /// </summary>
    string RelativePath { get; }
    
    /// <summary>
    /// The name of the generated type (interface, class, etc.).
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// The original namespace from the source.
    /// </summary>
    string? Namespace { get; }
}
