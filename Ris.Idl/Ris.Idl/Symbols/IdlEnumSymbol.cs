using Ris.Idl.Core;
using Ris.Idl.Symbols.Members;
using Ris.Idl.Utilities;

namespace Ris.Idl.Symbols;

/// <summary>
/// The symbol for an enum.
/// </summary>
public class IdlEnumSymbol : IdlNamedSymbol
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
    
    /// <summary>
    /// The fields of the enum.
    /// </summary>
    public IReadOnlyList<IdlEnumFieldSymbol>? FieldSymbols { get; set; }

    protected bool Equals(IdlEnumSymbol other)
    {
        var equal = Id == other.Id && 
               Name == other.Name && 
               Namespace == other.Namespace && 
               Visibility == other.Visibility &&
               ((DocComment is null && other.DocComment is null) || DocComment?.Equals(other.DocComment) == true);
        
        if(!equal)
        {
            return false;
        }

        equal = ComparerUtility.Compare(FieldSymbols, other.FieldSymbols);
        
        return equal;
    }

    /// <inheritdoc />
    public override bool Equals(IIdlSymbol? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((IdlEnumSymbol)obj);
    }
}