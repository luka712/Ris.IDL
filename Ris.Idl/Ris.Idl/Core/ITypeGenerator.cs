using Microsoft.CodeAnalysis;
using Ris.Idl.Symbols;

namespace Ris.Idl.Core;

/// <summary>
/// Generates code for a specific type (interface, class, enum, etc.).
/// </summary>
public interface ITypeGenerator
{
    /// <summary>
    /// Generates code for the given type.
    /// </summary>
    /// <param name="type">The named symbol type.</param>
    /// <param name="config">The generator config.</param>
    GeneratedFile Generate(IIdlNamedSymbol type, GeneratorConfig? config = null);
    
    /// <summary>
    /// Checks if this generator can handle the given type.
    /// </summary>
    /// <param name="type">The <see cref="IIdlNamedSymbol"/>.</param>
    /// <returns><c>true</c> if file definition can be generated for a symbol.</returns>
    bool CanGenerate(IIdlNamedSymbol type);
}