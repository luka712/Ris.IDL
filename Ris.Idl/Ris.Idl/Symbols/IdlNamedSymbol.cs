using System.Text.Json.Serialization;
using Ris.Idl.Core;
using Ris.Idl.Symbols.DocComment;

namespace Ris.Idl.Symbols;

/// <summary>
/// The base class for all named symbols.
/// </summary>
public class IdlNamedSymbol : IIdlNamedSymbol
{
    /// <summary>
    /// The constructor.
    /// </summary>
    /// <param name="name">The name of the symbol.</param>
    /// <param name="namespace">The namespace of the symbol.</param>
    internal IdlNamedSymbol(string name, string @namespace)
    {
        Name = name;
        Namespace = @namespace;
        Id = IdGenerator.CreateId(this);    
        Type = GetType().Name;
    }

    /// <summary>
    /// The constructor.
    /// </summary>
    [JsonConstructor]
    internal IdlNamedSymbol()
    {
        Name = string.Empty;
        Namespace = string.Empty;
        Id = string.Empty;
        Type = string.Empty;       
    }
    
    /// <inheritdoc />
    public string Id { get; set; }
    
    /// <inheritdoc />
    public string Name { get; set; }

    /// <summary>
    /// The type of the symbol.
    /// </summary>
    public string Type { get; set; }

    /// <inheritdoc />
    public IdlDocCommentSymbol? DocComment { get; set; }
    
    /// <inheritdoc />
    public string Namespace { get; set; }
    
    /// <inheritdoc />
    public IdlVisibility Visibility { get; set; } = IdlVisibility.PRIVATE;

    /// <summary>
    /// The equality operator.
    /// </summary>
    /// <param name="other">The other named symbol</param>
    /// <returns><c>true</c> if properties are matching.</returns>
    protected virtual bool Equals(IdlNamedSymbol other)
    {
        return Id == other.Id 
               && Name == other.Name 
               && ((DocComment is null && other.DocComment is null) || DocComment?.Equals(other.DocComment) == true) 
               && Namespace == other.Namespace 
               && Visibility == other.Visibility;
    }

    /// <inheritdoc />
    public virtual bool Equals(IIdlSymbol? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((IdlNamedSymbol)obj);
    }
}