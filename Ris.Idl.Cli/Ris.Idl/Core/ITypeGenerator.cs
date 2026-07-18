using Microsoft.CodeAnalysis;

namespace Ris.Idl.Core;

/// <summary>
/// Generates code for a specific type (interface, class, enum, etc.).
/// </summary>
public interface ITypeGenerator
{
    /// <summary>
    /// Converts a C# type symbol to a generated file.
    /// </summary>
    /// <param name="type">The type symbol to convert.</param>
    /// <param name="config">Optional configuration.</param>
    /// <returns>The generated file.</returns>
    IGeneratedFile Generate(INamedTypeSymbol type, GeneratorConfig? config = null);
    
    /// <summary>
    /// Checks if this generator can handle the given type.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>True if this generator can handle the type.</returns>
    bool CanGenerate(INamedTypeSymbol type);
}
