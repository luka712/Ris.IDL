namespace Ris.Idl.Core;

/// <summary>
/// This is a custom project type for generated idl
/// that provides interfaces, headers only without any runtime
/// information.
/// </summary>
public class GeneratedIdlProject
{
    /// <summary>
    /// Creates a new generated project.
    /// </summary>
    /// <param name="files">The generated files.</param>
    /// <param name="configuration">The project configuration.</param>
    public GeneratedIdlProject(IReadOnlyList<IGeneratedFile> files, IdlProjectConfiguration? configuration = null)
    {
        Files = files;
        Configuration = configuration ?? new IdlProjectConfiguration();
    }
    
    /// <summary>
    /// All generated files in the project.
    /// </summary>
    public IReadOnlyList<IGeneratedFile> Files { get; }
    
    /// <summary>
    /// The project configuration.
    /// </summary>
    public IdlProjectConfiguration Configuration { get; }

    /// <summary>
    /// Additional project files (package.json, tsconfig.json, etc.).
    /// </summary>
    public Dictionary<string, string> ProjectFiles { get; } = new();
}
