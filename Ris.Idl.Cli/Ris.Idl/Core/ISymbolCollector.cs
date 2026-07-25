using Microsoft.CodeAnalysis;
using Ris.Idl.Symbols;

namespace Ris.Idl.Core;

/// <summary>
/// Symbol collector is part of the pipeline that collects all symbols from a project
/// and converts them into a list of IIdlSymbol which can be used further to generate desired
/// language-specific files.
/// </summary>
public interface ISymbolCollector
{
    /// TODO: 
    IReadOnlyList<IIdlSymbol> CollectSymbols(IEnumerable<INamedTypeSymbol> symbols);
}