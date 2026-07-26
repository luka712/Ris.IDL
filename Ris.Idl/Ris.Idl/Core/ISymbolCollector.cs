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
    /// <summary>
    /// Collects all named symbols from a project.
    /// This includes classes, enums, interfaces, etc.
    /// </summary>
    /// <param name="symbols">The roslyn compiler symbols.</param>
    /// <returns>The Idl symbols.</returns>
    IReadOnlyList<IIdlNamedSymbol> CollectSymbols(IEnumerable<INamedTypeSymbol> symbols);
}