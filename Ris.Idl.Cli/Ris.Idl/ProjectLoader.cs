using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Ris.Idl.Core;
using Ris.Idl.TypeScript;

namespace Ris.Idl;

/// <summary>
/// Loads C# projects and generates code for target languages.
/// </summary>
public class ProjectLoader
{
    private static bool _msBuildRegistered;
    private static readonly object _lock = new();

    private readonly List<ITypeGenerator> _generators = new();

    /// <summary>
    /// Creates a new project loader with default TypeScript generators.
    /// </summary>
    public ProjectLoader()
    {
        // Register default generators
        _generators.Add(new TypeScriptInterfaceGenerator());
        _generators.Add(new TypeScriptClassGenerator());
    }

    /// <summary>
    /// Adds a custom type generator.
    /// </summary>
    /// <param name="generator">The generator to add.</param>
    public void AddGenerator(ITypeGenerator generator)
    {
        _generators.Add(generator);
    }

    /// <summary>
    /// Loads a C# project and generates TypeScript files.
    /// </summary>
    /// <param name="projectPath">Path to the .csproj file.</param>
    /// <param name="config">Optional generator configuration.</param>
    /// <returns>A list of generated files.</returns>
    public async Task<IReadOnlyList<IGeneratedFile>> LoadProjectAsync(string projectPath, GeneratorConfig? config = null)
    {
        if (!File.Exists(projectPath))
        {
            throw new FileNotFoundException("Project file not found", projectPath);
        }

        EnsureMSBuildRegistered();

        config ??= new TypeScriptConfig();

        using var workspace = MSBuildWorkspace.Create();
        
        workspace.RegisterWorkspaceFailedHandler(e =>
        {
            if (e.Diagnostic.Kind == WorkspaceDiagnosticKind.Failure)
            {
                Console.Error.WriteLine($"Workspace error: {e.Diagnostic.Message}");
            }
        });

        var project = await workspace.OpenProjectAsync(projectPath);
        var compilation = await project.GetCompilationAsync();

        if (compilation is null)
        {
            throw new InvalidOperationException("Failed to compile project");
        }

        var generatedFiles = new List<IGeneratedFile>();

        // Process all types in the compilation
        foreach (var type in GetAllTypes(compilation))
        {
            var generator = _generators.FirstOrDefault(g => g.CanGenerate(type));
            if (generator != null)
            {
                try
                {
                    var file = generator.Generate(type, config);
                    generatedFiles.Add(file);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error generating {type.Name}: {ex.Message}");
                }
            }
        }

        return generatedFiles;
    }

    /// <summary>
    /// Loads a C# project and generates a complete TypeScript project.
    /// </summary>
    /// <param name="projectPath">Path to the .csproj file.</param>
    /// <param name="configuration">The project configuration.</param>
    /// <returns>The generated project.</returns>
    public async Task<GeneratedProject> GenerateProjectAsync(string projectPath, TypeScriptProjectConfiguration configuration)
    {
        var files = await LoadProjectAsync(projectPath, configuration.GeneratorConfig);
        var projectGenerator = new TypeScriptProjectGenerator();
        return await projectGenerator.GenerateAsync(configuration, files);
    }

    /// <summary>
    /// Loads a C# project and writes a complete TypeScript project to disk.
    /// </summary>
    /// <param name="projectPath">Path to the .csproj file.</param>
    /// <param name="configuration">The project configuration.</param>
    public async Task GenerateAndWriteProjectAsync(string projectPath, TypeScriptProjectConfiguration configuration)
    {
        var project = await GenerateProjectAsync(projectPath, configuration);
        var projectGenerator = new TypeScriptProjectGenerator();
        await projectGenerator.WriteProjectAsync(project);
    }

    /// <summary>
    /// Ensures MSBuild is registered (only once per process).
    /// </summary>
    private static void EnsureMSBuildRegistered()
    {
        if (_msBuildRegistered) return;

        lock (_lock)
        {
            if (_msBuildRegistered) return;
            
            MSBuildLocator.RegisterDefaults();
            _msBuildRegistered = true;
        }
    }

    /// <summary>
    /// Gets all types from a compilation.
    /// </summary>
    private static IEnumerable<INamedTypeSymbol> GetAllTypes(Compilation compilation)
    {
        return GetTypesFromNamespace(compilation.Assembly.GlobalNamespace);
    }

    /// <summary>
    /// Recursively gets all types from a namespace.
    /// </summary>
    private static IEnumerable<INamedTypeSymbol> GetTypesFromNamespace(INamespaceSymbol ns)
    {
        foreach (var type in ns.GetTypeMembers())
        {
            yield return type;

            // Get nested types
            foreach (var nested in GetNestedTypes(type))
            {
                yield return nested;
            }
        }

        foreach (var childNs in ns.GetNamespaceMembers())
        {
            foreach (var type in GetTypesFromNamespace(childNs))
            {
                yield return type;
            }
        }
    }

    /// <summary>
    /// Gets nested types from a type.
    /// </summary>
    private static IEnumerable<INamedTypeSymbol> GetNestedTypes(INamedTypeSymbol type)
    {
        foreach (var nested in type.GetTypeMembers())
        {
            yield return nested;

            foreach (var deepNested in GetNestedTypes(nested))
            {
                yield return deepNested;
            }
        }
    }
}
