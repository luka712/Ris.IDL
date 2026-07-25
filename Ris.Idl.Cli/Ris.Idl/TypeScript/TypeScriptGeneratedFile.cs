using Ris.Idl.Core;
using Ris.Idl.Symbols;

namespace Ris.Idl.TypeScript;

/// <summary>
/// Represents a generated TypeScript file.
/// </summary>
public class TypeScriptGeneratedFile : IGeneratedFile
{
    /// <summary>
    /// Creates a new TypeScript generated file.
    /// </summary>
    /// <param name="name">The name of the type.</param>
    /// <param name="content">The generated content.</param>
    /// <param name="relativePath">The relative file path.</param>
    /// <param name="namespace">The original namespace.</param>
    public TypeScriptGeneratedFile(string name, string content, string relativePath, string? @namespace = null)
    {
        Name = name;
        Content = content;
        RelativePath = relativePath;
        Namespace = @namespace;
    }

    /// <summary>
    /// The symbol that was generated.
    /// </summary>
    public IIdlSymbol Symbol { get; }

    /// <inheritdoc />
    public string Content { get; }
    
    /// <inheritdoc />
    public string RelativePath { get; }
    
    /// <inheritdoc />
    public string Name { get; }
    
    /// <inheritdoc />
    public string? Namespace { get; }
}
