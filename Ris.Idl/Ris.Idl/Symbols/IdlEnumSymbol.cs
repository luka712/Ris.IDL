using Ris.Idl.Core;
using Ris.Idl.Symbols.DocComment;
using Ris.Idl.Symbols.Members;

namespace Ris.Idl.Symbols;

/// <summary>
/// The symbol for an enum.
/// </summary>
public class IdlEnumSymbol : IIdlNamedSymbol
{
    /// <summary>
    /// The constructor.
    /// </summary>
    /// <param name="name">The name of the enum.</param>
    /// <param name="namespace">The enum namespace.</param>
    public IdlEnumSymbol(string name, string @namespace)
    {
        Name = name;
        Namespace = @namespace;
        Id = IdGenerator.CreateId(this);
    }

    public string Id { get; }

    /// <summary>
    /// The name of the enum.
    /// </summary>
    public string Name { get; }
    
    /// <inheritdoc />
    public string Namespace { get; }
    
    /// <summary>
    /// The visibility of the enum.
    /// </summary>
    public IdlVisibility Visibility { get; set; } = IdlVisibility.PRIVATE;
    
    /// <summary>
    /// The documentation comment for the interface.
    /// </summary>
    public IdlDocCommentSymbol? DocComment { get; set; }
    
    /// <summary>
    /// The fields of the enum.
    /// </summary>
    public IReadOnlyList<IdlEnumFieldSymbol>? FieldSymbols { get; set; }

    protected bool Equals(IdlEnumSymbol other)
    {
        return Id == other.Id && 
               Name == other.Name && 
               Namespace == other.Namespace && 
               Visibility == other.Visibility &&
               Equals(DocComment, other.DocComment) &&
               Equals(FieldSymbols, other.FieldSymbols);
    }

    /// <inheritdoc />
    public bool Equals(IIdlSymbol? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((IdlEnumSymbol)obj);
    }
}