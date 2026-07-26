using Ris.Idl.Core;

namespace Ris.Idl.TypeScript.Configuration;

/// <summary>
/// TypeScript-specific project configuration.
/// </summary>
public class TypeScriptProjectConfiguration : ProjectConfiguration
{
    public TypeScriptProjectConfiguration()
    {
        GeneratorConfig = new TypeScriptConfig();
    }
    
    /// <summary>
    /// The TypeScript configuration (convenience accessor).
    /// </summary>
    public TypeScriptConfig TypeScriptConfig => (TypeScriptConfig)GeneratorConfig;
    
    /// <summary>
    /// Whether to generate a package.json file.
    /// </summary>
    public bool GeneratePackageJson { get; set; } = true;
    
    /// <summary>
    /// Whether to generate a tsconfig.json file.
    /// </summary>
    public bool GenerateTsConfig { get; set; } = true;
    
    /// <summary>
    /// Whether to generate an index.ts barrel file that exports all types.
    /// </summary>
    public bool GenerateIndexFile { get; set; } = true;
    
    /// <summary>
    /// The TypeScript target version (e.g., "ES2020", "ES2022", "ESNext").
    /// </summary>
    public string TypeScriptTarget { get; set; } = "ES2022";
    
    /// <summary>
    /// The module system to use (e.g., "ESNext", "CommonJS", "NodeNext").
    /// </summary>
    public string ModuleSystem { get; set; } = "ESNext";
    
    /// <summary>
    /// Whether to enable strict mode in TypeScript.
    /// </summary>
    public bool StrictMode { get; set; } = true;
    
    /// <summary>
    /// Whether to generate declaration files (.d.ts).
    /// </summary>
    public bool GenerateDeclarations { get; set; } = true;
    
    /// <summary>
    /// Additional npm dependencies to include in package.json.
    /// </summary>
    public Dictionary<string, string> Dependencies { get; set; } = new();
    
    /// <summary>
    /// Additional npm dev dependencies to include in package.json.
    /// </summary>
    public Dictionary<string, string> DevDependencies { get; set; } = new()
    {
        ["typescript"] = "^5.0.0"
    };
    
    /// <summary>
    /// The license for the package.
    /// </summary>
    public string? License { get; set; } = "MIT";
    
    /// <summary>
    /// Keywords for the package.
    /// </summary>
    public List<string> Keywords { get; set; } = new() { "types", "typescript", "generated" };
    
    /// <summary>
    /// The repository URL.
    /// </summary>
    public string? Repository { get; set; }
}
