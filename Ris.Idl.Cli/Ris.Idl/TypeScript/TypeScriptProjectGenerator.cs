using System.Text;
using System.Text.Json;
using Ris.Idl.Core;

namespace Ris.Idl.TypeScript;

/// <summary>
/// Generates a complete TypeScript project from generated files.
/// </summary>
public class TypeScriptProjectGenerator : IProjectGenerator
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <inheritdoc />
    public Task<GeneratedProject> GenerateAsync(ProjectConfiguration configuration, IReadOnlyList<IGeneratedFile> files)
    {
        var tsConfig = configuration as TypeScriptProjectConfiguration ?? new TypeScriptProjectConfiguration
        {
            Name = configuration.Name,
            Version = configuration.Version,
            Description = configuration.Description,
            Author = configuration.Author,
            OutputDirectory = configuration.OutputDirectory,
            GeneratorConfig = configuration.GeneratorConfig
        };

        var project = new GeneratedProject(files, tsConfig);

        // Generate package.json
        if (tsConfig.GeneratePackageJson)
        {
            project.ProjectFiles["package.json"] = GeneratePackageJson(tsConfig);
        }

        // Generate tsconfig.json
        if (tsConfig.GenerateTsConfig)
        {
            project.ProjectFiles["tsconfig.json"] = GenerateTsConfig(tsConfig);
        }

        // Generate index.ts barrel file
        if (tsConfig.GenerateIndexFile)
        {
            project.ProjectFiles["src/index.ts"] = GenerateIndexFile(files, tsConfig);
        }

        return Task.FromResult(project);
    }

    /// <inheritdoc />
    public async Task WriteProjectAsync(GeneratedProject project)
    {
        var outputDir = project.Configuration.OutputDirectory;
        
        // Ensure output directory exists
        Directory.CreateDirectory(outputDir);

        // Write all generated type files
        foreach (var file in project.Files)
        {
            var filePath = Path.Combine(outputDir, file.RelativePath);
            var directory = Path.GetDirectoryName(filePath);
            
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllTextAsync(filePath, file.Content);
        }

        // Write project files (package.json, tsconfig.json, etc.)
        foreach (var (relativePath, content) in project.ProjectFiles)
        {
            var filePath = Path.Combine(outputDir, relativePath);
            var directory = Path.GetDirectoryName(filePath);
            
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllTextAsync(filePath, content);
        }
    }

    /// <summary>
    /// Generates the package.json content.
    /// </summary>
    private string GeneratePackageJson(TypeScriptProjectConfiguration config)
    {
        var package = new Dictionary<string, object>
        {
            ["name"] = config.Name,
            ["version"] = config.Version,
            ["type"] = "module",
            ["main"] = "./dist/index.js",
            ["types"] = "./dist/index.d.ts",
            ["exports"] = new Dictionary<string, object>
            {
                ["."] = new Dictionary<string, string>
                {
                    ["import"] = "./dist/index.js",
                    ["types"] = "./dist/index.d.ts"
                }
            },
            ["files"] = new[] { "dist", "src" },
            ["scripts"] = new Dictionary<string, string>
            {
                ["build"] = "tsc",
                ["clean"] = "rm -rf dist",
                ["prepublishOnly"] = "npm run build"
            }
        };

        if (!string.IsNullOrEmpty(config.Description))
        {
            package["description"] = config.Description;
        }

        if (!string.IsNullOrEmpty(config.Author))
        {
            package["author"] = config.Author;
        }

        if (!string.IsNullOrEmpty(config.License))
        {
            package["license"] = config.License;
        }

        if (config.Keywords.Count > 0)
        {
            package["keywords"] = config.Keywords;
        }

        if (!string.IsNullOrEmpty(config.Repository))
        {
            package["repository"] = new Dictionary<string, string>
            {
                ["type"] = "git",
                ["url"] = config.Repository
            };
        }

        if (config.Dependencies.Count > 0)
        {
            package["dependencies"] = config.Dependencies;
        }

        if (config.DevDependencies.Count > 0)
        {
            package["devDependencies"] = config.DevDependencies;
        }

        return JsonSerializer.Serialize(package, JsonOptions);
    }

    /// <summary>
    /// Generates the tsconfig.json content.
    /// </summary>
    private string GenerateTsConfig(TypeScriptProjectConfiguration config)
    {
        var tsconfig = new Dictionary<string, object>
        {
            ["compilerOptions"] = new Dictionary<string, object>
            {
                ["target"] = config.TypeScriptTarget,
                ["module"] = config.ModuleSystem,
                ["moduleResolution"] = config.ModuleSystem == "NodeNext" ? "NodeNext" : "bundler",
                ["strict"] = config.StrictMode,
                ["declaration"] = config.GenerateDeclarations,
                ["declarationMap"] = config.GenerateDeclarations,
                ["sourceMap"] = true,
                ["outDir"] = "./dist",
                ["rootDir"] = "./src",
                ["esModuleInterop"] = true,
                ["skipLibCheck"] = true,
                ["forceConsistentCasingInFileNames"] = true
            },
            ["include"] = new[] { "src/**/*" },
            ["exclude"] = new[] { "node_modules", "dist" }
        };

        return JsonSerializer.Serialize(tsconfig, JsonOptions);
    }

    /// <summary>
    /// Generates the index.ts barrel file that exports all types.
    /// </summary>
    private string GenerateIndexFile(IReadOnlyList<IGeneratedFile> files, TypeScriptProjectConfiguration config)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// Auto-generated barrel file - exports all types");
        sb.AppendLine();

        // Group files by namespace/module for organized exports
        var filesByNamespace = files
            .GroupBy(f => f.Namespace ?? "")
            .OrderBy(g => g.Key);

        foreach (var group in filesByNamespace)
        {
            if (!string.IsNullOrEmpty(group.Key))
            {
                sb.AppendLine($"// {group.Key}");
            }

            foreach (var file in group.OrderBy(f => f.Name))
            {
                // Convert relative path to import path (remove src/ prefix and .ts extension)
                var importPath = file.RelativePath
                    .Replace("src/", "./")
                    .Replace(".ts", "");
                
                sb.AppendLine($"export * from '{importPath}';");
            }

            sb.AppendLine();
        }

        return sb.ToString().TrimEnd() + Environment.NewLine;
    }
}
