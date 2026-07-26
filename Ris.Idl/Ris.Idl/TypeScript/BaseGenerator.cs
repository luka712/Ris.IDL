using Ris.Idl.Symbols;
using Ris.Idl.Symbols.Members;

namespace Ris.Idl.TypeScript;

public class BaseGenerator
{
    public IReadOnlyList<IdlTypeSymbol> TraverseRequiredTypes(IdlInterfaceSymbol symbol)
    {
        List<IdlTypeSymbol> namespaces = symbol.PropertySymbols
            ?.Select(x => x.TypeSymbol)
            .Distinct()
            .ToList() ?? new();

        return namespaces.Distinct().ToList();
    }
}