using Ris.Idl.Symbols;

namespace Ris.Idl.Core;

/// <summary>
/// Represents a generated file with its content and path.
/// </summary>
public class GeneratedFile
{
    /// TODO:
    public GeneratedFile(IIdlNamedSymbol symbol, string content, string relativePath)
    {
        Symbol = symbol;
        RelativePath = relativePath;
        Content = content;
    }
    
    /// <summary>
    /// The symbol of this file.
    /// </summary>
    public IIdlNamedSymbol Symbol { get; }
    
    /// <summary>
    /// The generated source code content.
    /// </summary>
    public string Content { get; }
    
    /// <summary>
    /// The relative file path where this file should be written.
    /// </summary>
    public string RelativePath { get; }
    
}
