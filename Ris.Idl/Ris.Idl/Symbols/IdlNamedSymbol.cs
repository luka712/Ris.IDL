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
    }
    
    /// <inheritdoc />
    public string Id { get; }
    
    /// <inheritdoc />
    public string Name { get; }
    
    /// <inheritdoc />
    public IdlDocCommentSymbol? DocComment { get; set; }
    
    /// <inheritdoc />
    public string Namespace { get; }
    
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
               && Equals(DocComment, other.DocComment) 
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