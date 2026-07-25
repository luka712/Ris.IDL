using Ris.Idl.Symbols.DocComment;

namespace Ris.Idl.Symbols.Members;

/// <summary>
/// The symbol for a method parameter.
/// </summary>
public record IdlMethodParameterSymbol
{
    /// <summary>
    /// The name of the parameter.
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// The type of the parameter.
    /// </summary>
    public required string Type { get; set; }
    
    /// <summary>
    /// The default value of the parameter.
    /// </summary>
    public object? DefaultValue { get; set; }
}