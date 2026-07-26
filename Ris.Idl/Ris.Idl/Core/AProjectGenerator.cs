using Ris.Idl.Generated;
using Ris.Idl.Symbols;

namespace Ris.Idl.Core;

/// <summary>
/// Generates a complete project from a collection of generated files.
/// </summary>
public abstract class AProjectGenerator
{
    /// <summary>
    /// Generates a complete project structure.
    /// </summary>
    /// <param name="configuration">The project configuration.</param>
    /// <param name="idlProject">TODO</param>
    /// <returns>The generated project.</returns>
    public abstract Task<GeneratedProject> GenerateProjectAsync(ProjectConfiguration configuration, IdlProject idlProject);
    
    /// <summary>
    /// Writes the generated project to disk.
    /// </summary>
    /// <param name="project">The generated project.</param>
    public abstract Task WriteProjectAsync(GeneratedProject project);

    /// <summary>
    /// Generates the symbol meta-JSON file.
    /// </summary>
    /// <param name="symbols">The <see cref="IdlProjectSymbols"/>.</param>
    /// <returns>The JSON content.</returns>
    protected string GenerateSymbolsMetaJson(IdlProjectSymbols symbols)
    {
        return symbols.ToJson();
    }
}
