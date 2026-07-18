namespace Ris.Idl.Interfaces;

/// <summary>
/// The project generator whicvh is responsible for generating the project
/// from a C# read project, to other types such as TypeScript.
/// </summary>
public interface IProjectGenerator
{
    /// <summary>
    /// Generate the project.
    /// </summary>
    /// <param name="configuration">The <see cref="IProjectConfiguration"/>.</param>
    /// <param name="interfacesCollection">The collection of interfaces.</param>
    Task<Project> GenerateAsync(IProjectConfiguration configuration, IReadOnlyList<IInterface> interfacesCollection);
}