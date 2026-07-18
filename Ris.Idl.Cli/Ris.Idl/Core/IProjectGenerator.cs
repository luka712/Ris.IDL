namespace Ris.Idl.Core;

/// <summary>
/// Generates a complete project from a collection of generated files.
/// </summary>
public interface IProjectGenerator
{
    /// <summary>
    /// Generates a complete project structure.
    /// </summary>
    /// <param name="configuration">The project configuration.</param>
    /// <param name="files">The generated files to include.</param>
    /// <returns>The generated project.</returns>
    Task<GeneratedProject> GenerateAsync(ProjectConfiguration configuration, IReadOnlyList<IGeneratedFile> files);
    
    /// <summary>
    /// Writes the generated project to disk.
    /// </summary>
    /// <param name="project">The generated project.</param>
    Task WriteProjectAsync(GeneratedProject project);
}
