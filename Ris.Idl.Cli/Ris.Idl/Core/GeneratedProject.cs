using Ris.Idl.Symbols;

namespace Ris.Idl.Core;

/// <summary>
/// Represents a generated project with all its files.
/// </summary>
public class GeneratedProject
{
    /// <summary>
    /// Creates a new generated project.
    /// </summary>
    /// <param name="files">The generated files.</param>
    /// <param name="configuration">The project configuration.</param>
    public GeneratedProject(IReadOnlyList<IGeneratedFile> files, ProjectConfiguration configuration)
    {
        Files = files;
        Configuration = configuration;
    }
    
    /// <summary>
    /// All generated files in the project.
    /// </summary>
    public IReadOnlyList<IGeneratedFile> Files { get; }
    
    /// <summary>
    /// The project configuration.
    /// </summary>
    public ProjectConfiguration Configuration { get; }
    
    /// <summary>
    /// Additional project files (package.json, tsconfig.json, etc.).
    /// </summary>
    public Dictionary<string, string> ProjectFiles { get; } = new();
}
