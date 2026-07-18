using Microsoft.CodeAnalysis;

namespace Ris.Idl.Interfaces;

public interface IInterfaceGenerator
{

    // IInterface Convert(string source, Config? config = null);

    IInterface Convert(INamedTypeSymbol type, Config? config = null);
}