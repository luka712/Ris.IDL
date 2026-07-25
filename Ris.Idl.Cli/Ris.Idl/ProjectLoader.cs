using Windows.Devices.Midi;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Extensions.Logging;
using Ris.Idl.Core;
using Ris.Idl.Symbols;
using Ris.Idl.TypeScript;
using Ris.Idl.TypeScript.Configuration;

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
        var logger = LoggerFactory
            .Create(builder => builder.SetMinimumLevel(LogLevel.Trace).AddConsole())
            .CreateLogger<ProjectLoader>();
        logger.LogInformation("Initializing project loader");
        
        // Register default generators
        _generators.Add(new TypeScriptInterfaceGenerator(logger));
        _generators.Add(new TypeScriptClassGenerator(logger));
        _generators.Add(new TypeScriptEnumsGenerator(logger));
    }

    /// <summary>
    /// The symbol collector.
    /// </summary>
    public ISymbolCollector SymbolCollector { get; set; } = new SymbolCollector();

    /// <summary>
    /// Adds a custom type generator.
    /// </summary>
    /// <param name="generator">The generator to add.</param>
    public void AddGenerator(ITypeGenerator generator)
    {
        _generators.Add(generator);
    }

    private async Task<IReadOnlyList<INamedTypeSymbol>> ReadProjectSymbolsAsync(string projectPath)
    {
        if (!File.Exists(projectPath))
        {
            throw new FileNotFoundException("Project file not found", projectPath);
        }

        EnsureMSBuildRegistered();
        
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
        
        return GetTypesFromNamespace(compilation.Assembly.GlobalNamespace).ToList();
    }
    
    /// <summary>
    /// Reads a project and returns <see cref="IdlProject"/>.
    /// </summary>
    /// <param name="projectPath">The path to the project.</param>
    /// <returns>The <see cref="IdlProject"/>.</returns>
    public async Task<IdlProject> ReadProjectAsync(string projectPath)
    {
        // 1. Collect all symbols from the project.
        var roslynSymbols = await ReadProjectSymbolsAsync(projectPath);
        
        // 2. Now we convert that to idl symbols.
        var idlSymbols = SymbolCollector.CollectSymbols(roslynSymbols);

        List<IdlInterfaceSymbol> interfaces = new();
        List<IdlClassSymbol> classes = new();
        
        foreach (var symbol in idlSymbols)
        {
            if (symbol is IdlInterfaceSymbol interfaceSymbol)
            {
                interfaces.Add(interfaceSymbol);
            }
            
            if (symbol is IdlClassSymbol classSymbol)
            {
                classes.Add(classSymbol);
            }
        }
        
        return new IdlProject()
        {
            Classes = classes,
            Interfaces = interfaces
        };
    }

    /// <summary>
    /// Loads a C# project and generates TypeScript files.
    /// </summary>
    /// <param name="projectPath">Path to the .csproj file.</param>
    /// <param name="config">Optional generator configuration.</param>
    /// <returns>A list of generated files.</returns>
    public async Task<IReadOnlyList<IGeneratedFile>> LoadProjectAsync(string projectPath, GeneratorConfig? config = null)
    {
        // 2. Now we need to collect all symbols from the compilation.
        var roslynSymbols = await ReadProjectSymbolsAsync(projectPath);
        var idlSymbols = SymbolCollector.CollectSymbols(roslynSymbols);

        var generatedFiles = new List<IGeneratedFile>();

        // Process all types in the compilation
        foreach (var type in roslynSymbols)
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

    private void CollectTypes(Compilation compilation, List<INamedTypeSymbol> types)
    {
        
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
        return await projectGenerator.GenerateProjectAsync(configuration, files);
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
