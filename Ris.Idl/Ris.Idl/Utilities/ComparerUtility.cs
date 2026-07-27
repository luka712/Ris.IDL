using System.Reflection.Metadata;
using Ris.Idl.Symbols;
using Ris.Idl.Symbols.DocComment;

namespace Ris.Idl.Utilities;

// TODO
internal  static class ComparerUtility
{
    // TODO:
    internal  static bool Compare(IReadOnlyList<IIdlSymbol>? a, IReadOnlyList<IIdlSymbol>? b)
    {
        // If both are null, then they are equal.
        if (a is null && b is null)
        {
            return true;
        }
        
        // If they are not null, then they must be compared per element.
        if(a is not null && b is not null)
        {
            // First check count.
            if (a.Count != b.Count)
            {
                return false;
            }
            
            foreach(var symbol in a)
            {
                // If the symbol is not in the other list, then it is not equal.
                if (!b.Any(x => x.Equals(symbol)))
                {
                    return false;
                }
            }
            
            return true;
        }

        // One is null, the other is not.
        return false;
    }

    // TODO:
    internal static bool Compare(IdlDocCommentSymbol? a, IdlDocCommentSymbol? b)
    {
        if (a is null && b is null)
        {
            return true;
        }
        
        return a?.Equals(b) == true;
    }
}