using Ris.Idl.Symbols.DocComment;
using Ris.Idl.Utilities;

namespace Ris.Idl.Symbols.Members;

public record IdlEnumFieldSymbol : IIdlSymbol
{
    /// <summary>
    /// The name of the key case.
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// The type of the key case.
    /// </summary>
    public string? TypeName { get; set; }
    
    /// <summary>
    /// The value of the key case.
    /// </summary>
    public string? Value { get; set; }
    
    /// <summary>
    /// The documentation comment for the key case.
    /// </summary>
    public IdlDocCommentSymbol? DocComment { get; set; }


    public virtual bool Equals(IIdlSymbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        if (other is IdlEnumFieldSymbol o)
        {
            return Name == o.Name
                   && TypeName == o.TypeName
                   && Value == o.Value 
                   && ComparerUtility.Compare(DocComment, o.DocComment);
        }
        
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, TypeName, Value, DocComment);
    }
}