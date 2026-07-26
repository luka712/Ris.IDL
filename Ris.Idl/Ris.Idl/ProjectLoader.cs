using Microsoft.Extensions.Logging;
using Ris.Idl.Core;
using Ris.Idl.Generated;
using Ris.Idl.Roslyn;
using Ris.Idl.Symbols;
using Ris.Idl.TypeScript;
using Ris.Idl.TypeScript.Configuration;

namespace Ris.Idl;

/// <summary>
/// Loads C# projects and generates code for target languages.
/// </summary>
public class ProjectLoader
{
    private readonly RoslynCompilerService _roslynCompilerService = new();
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
        _generators.Add(new TypeScriptStructGenerator(logger));
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

    /// <summary>
    /// Reads a project and returns <see cref="IdlProjectSymbols"/>.
    /// </summary>
    /// <param name="projectPath">The path to the project.</param>
    /// <returns>The <see cref="IdlProjectSymbols"/>.</returns>
    public async Task<IdlProjectSymbols> ReadProjectAsync(string projectPath)
    {
        // 1. Load the project and collect all symbols from the project.
        var roslynSymbols = await _roslynCompilerService.ReadRoslynSymbolsAsync(projectPath);

        // 2. Now we convert that to idl symbols.
        var idlSymbols = SymbolCollector.CollectSymbols(roslynSymbols);

        List<IdlInterfaceSymbol> interfaces = new();
        List<IdlClassSymbol> classes = new();
        List<IdlEnumSymbol> enums = new();
        List<IdlStructSymbol> structs = new();

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

            if (symbol is IdlEnumSymbol enumSymbol)
            {
                enums.Add(enumSymbol);
            }

            if (symbol is IdlStructSymbol structSymbol)
            {
                structs.Add(structSymbol);
            }
        }

        return new IdlProjectSymbols()
        {
            Classes = classes,
            Interfaces = interfaces,
            Enums = enums,
            Structs = structs,
        };
    }

    /// <summary>
    /// Loads a C# project and generates TypeScript files.
    /// </summary>
    /// <param name="projectPath">Path to the .csproj file.</param>
    /// <param name="config">Optional generator configuration.</param>
    /// <returns>A list of generated files.</returns>
    public async Task<IdlProject> LoadProjectAsync(
        string projectPath,
        GeneratorConfig? config = null)
    {
        // 1. Read the project symbols.
        var symbolsProject = await ReadProjectAsync(projectPath);

        // 2. Generate TypeScript files.
        var generatedFiles = new List<GeneratedFile>();

        foreach (var symbol in symbolsProject.GetAllSymbols())
        {
            var generator = _generators.FirstOrDefault(g => g.CanGenerate(symbol));

            if (generator != null)
            {
                var file = generator.Generate(symbol, config);
                generatedFiles.Add(file);
            }
        }

        return new IdlProject()
        {
            GeneratedFiles = generatedFiles,
            Symbols = symbolsProject
        };
    }
    
    /// <summary>
    /// Loads a C# project and generates a complete TypeScript project.
    /// </summary>
    /// <param name="projectPath">Path to the .csproj file.</param>
    /// <param name="configuration">The project configuration.</param>
    /// <returns>The generated project.</returns>
    public async Task<GeneratedProject> GenerateProjectAsync(string projectPath,
        TypeScriptProjectConfiguration configuration)
    {
        var idlProject = await LoadProjectAsync(projectPath, configuration.GeneratorConfig);
        var projectGenerator = new TypeScriptProjectGenerator();
        return await projectGenerator.GenerateProjectAsync(configuration, idlProject);
    }
}